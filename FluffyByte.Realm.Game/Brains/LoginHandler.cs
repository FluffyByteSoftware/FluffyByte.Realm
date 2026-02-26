/*
 * (LoginHandler.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@12:29:20 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Shared.Misc;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains;

public static class LoginHandler
{
    private static bool _isInitialized;

    private const int CharacterSelectTimeoutSeconds = 30;

    #region Initialization

    public static void Initialize()
    {
        if (_isInitialized) return;

        EventManager.Subscribe<OnAuthenticationSuccessEvent>(OnAuthSuccess);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);

        _isInitialized = true;
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized) return;

        _isInitialized = false;

        EventManager.Unsubscribe<OnAuthenticationSuccessEvent>(OnAuthSuccess);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
    }

    #endregion Initialization

    #region Auth Flow

    private static void OnAuthSuccess(OnAuthenticationSuccessEvent e)
    {
        var account = AccountManager.GetAccountByUsername(e.AccountName);

        if (account == null)
        {
            Log.Warn($"[LoginHandler]: Account '{e.AccountName}' not found post-auth. Disconnecting.");
            e.Client.Disconnect();
            return;
        }

        SendCharacterList(e.Client, account);

        _ = WaitForCharacterActionAsync(e.Client, account);
    }

    #endregion Auth Flow

    #region Character List

    private static void SendCharacterList(RealmClient client, RealmAccount account)
    {
        var packet = new CharacterListPacket
        {
            SlotCount = account.Characters.Length,
            Slots = account.Characters.Select(guid =>
            {
                if (guid == Guid.Empty)
                    return CharacterSlot.Empty;

                var profile = GameDirector.GetPlayerProfile(guid);

                return profile == null
                    ? CharacterSlot.Empty
                    : new CharacterSlot { Id = profile.Id, Name = profile.Name };
            }).ToArray()
        };

        client.SendPacket(PacketType.CharacterList, packet);
    }

    #endregion Character List

    #region Character Action Loop

    private static async Task WaitForCharacterActionAsync(RealmClient client, RealmAccount account)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(CharacterSelectTimeoutSeconds);

        while (DateTime.UtcNow < deadline)
        {
            if (!client.IsConnected)
                return;

            // Check for character selection
            var selectPackets = client.DrainQueue(PacketType.SelectCharacter);
            if (selectPackets.Count > 0)
            {
                var selectPacket = (SelectCharacterPacket)selectPackets[0];
                HandleSelectCharacter(client, account, selectPacket);
                return;
            }

            // Check for character creation
            var createPackets = client.DrainQueue(PacketType.RequestCreateCharacter);
            if (createPackets.Count > 0)
            {
                var createPacket = (RequestCreateCharacterPacket)createPackets[0];
                HandleCreateCharacter(client, account, createPacket);

                // Reset deadline — player is still at character select
                deadline = DateTime.UtcNow + TimeSpan.FromSeconds(CharacterSelectTimeoutSeconds);
                continue;
            }

            // Check for character deletion
            var deletePackets = client.DrainQueue(PacketType.RequestDeleteCharacter);
            if (deletePackets.Count > 0)
            {
                var deletePacket = (RequestDeleteCharacterPacket)deletePackets[0];
                HandleDeleteCharacter(client, account, deletePacket);

                // Reset deadline — player is still at character select
                deadline = DateTime.UtcNow + TimeSpan.FromSeconds(CharacterSelectTimeoutSeconds);
                continue;
            }

            await Task.Delay(100);
        }

        Log.Warn($"[LoginHandler]: Character select timed out for {client.Name}. Disconnecting.");
        client.Disconnect();
    }

    #endregion Character Action Loop

    #region Select Character

    private static void HandleSelectCharacter(RealmClient client, 
        RealmAccount account, 
        SelectCharacterPacket packet)
    {
        if (!account.Characters.Contains(packet.CharacterGuid))
        {
            Log.Warn($"[LoginHandler]: {client.Name} tried to select character {packet.CharacterGuid} " +
                     $"not on their account.");
            client.Disconnect();
            return;
        }

        var profile = GameDirector.GetPlayerProfile(packet.CharacterGuid);

        if (profile == null)
        {
            Log.Warn($"[LoginHandler]: Character {packet.CharacterGuid} not found on disk. Disconnecting " +
                     $"{client.Name}.");
            client.Disconnect();
            return;
        }

        SpawnPlayer(client, profile);
    }

    #endregion Select Character

    #region Create Character

    private static void HandleCreateCharacter(RealmClient client, 
        RealmAccount account, 
        RequestCreateCharacterPacket packet)
    {
        var profile = PlayerCharacterManager.CreateCharacter(packet.Name, account);

        if (profile == null)
        {
            var validation = PlayerCharacterManager.ValidateName(packet.Name);

            var response = new CreateCharacterResponsePacket
            {
                Success = false,
                Reason = validation
            };

            client.SendPacket(PacketType.CreateCharacterResponse, response);
            return;
        }

        var successResponse = new CreateCharacterResponsePacket
        {
            Success = true,
            Reason = NameValidationResult.Valid
        };

        client.SendPacket(PacketType.CreateCharacterResponse, successResponse);

        // Resend updated character list
        SendCharacterList(client, account);
    }

    #endregion Create Character

    #region Delete Character

    private static void HandleDeleteCharacter(RealmClient client, 
        RealmAccount account, 
        RequestDeleteCharacterPacket packet)
    {
        var success = PlayerCharacterManager.DeleteCharacter(packet.CharacterId, account);

        var response = new DeleteCharacterResponsePacket
        {
            Success = success
        };

        client.SendPacket(PacketType.DeleteCharacterResponse, response);

        // Resend updated character list
        SendCharacterList(client, account);
    }

    #endregion Delete Character

    #region Spawn Player

    private static void SpawnPlayer(RealmClient client, PlayerProfile profile)
    {
        var template = GameDirector.ProfileToTemplate(profile);
        var actor = ActorFactory.CreatePlayerActor(template);

        var spawnTile = GameDirector.World.GetTile(profile.CurrentTileX, profile.CurrentTileZ);

        GameDirector.SpawnGameObject(actor, spawnTile);

        Log.Info($"[LoginHandler]: Spawned '{profile.Name}' for {client.Name} " +
                 $"at ({profile.CurrentTileX},{profile.CurrentTileZ}).");

        var spawnPacket = new CharacterSelectedPacket
        {
            Success = true,
            NetworkId = actor.NetworkId,
            GlobalX = spawnTile.GlobalX,
            GlobalZ = spawnTile.GlobalZ
        };

        client.SendPacket(PacketType.CharacterSelected, spawnPacket);
    }

    #endregion Spawn Player
}

/*
 *------------------------------------------------------------
 * (LoginHandler.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */