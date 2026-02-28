/*
 * (LoginHandler.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@12:29:20 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Networking.Server;
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

            ClientManager.RemoveRealmClient(e.Client);

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
            Slots = [.. account.Characters.Select(guid =>
            {
                if (guid == Guid.Empty)
                    return CharacterSlot.Empty;

                var profile = GameDirector.GetPlayerProfile(guid);

                return profile == null
                    ? CharacterSlot.Empty
                    : new CharacterSlot { Id = profile.Id, Name = profile.Name };
            })]
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
        
        ClientManager.RemoveRealmClient(client);
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
            ClientManager.RemoveRealmClient(client);
            return;
        }

        var profile = GameDirector.GetPlayerProfile(packet.CharacterGuid);

        if (profile == null)
        {
            Log.Warn($"[LoginHandler]: Character {packet.CharacterGuid} not found on disk. Disconnecting " +
                     $"{client.Name}.");
            ClientManager.RemoveRealmClient(client);
            return;
        }

        SpawnPlayer(client, profile).Wait();
    }

    #endregion Select Character

    #region Create Character

    private static void HandleCreateCharacter(RealmClient client, 
        RealmAccount account, 
        RequestCreateCharacterPacket packet)
    {
        var profile = new PlayerProfile
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Name = packet.Name,
            CurrentHealth = 100,
            MaxHealth = 100,
            CurrentTileX = 0,
            CurrentTileZ = 0,
            PreviousTileX = 0,
            PreviousTileZ = 0,
            LineOfSight = 350,
            AudibleRange = 200,
            Strength = 10,
            Dexterity = 10,
            Constitution = 10,
            Intelligence = 10,
            Wisdom = 10,
            Charisma = 10,
            HealthRegenPerTick = 1,
            HealthRegenIntervalSeconds = 5,
            HealthRegenMultiplier = 1,
            FootprintRadius = 1,
            ModelType = PrimitiveModelType.Capsule,
            ComplexModelType = ComplexModelType.DefaultAndrogynous
        };

        var result = PlayerCharacterManager.CreateCharacter(profile, account);

        if (result == null)
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

        account.Save();

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

    private static async Task SpawnPlayer(RealmClient client, PlayerProfile profile)
    {
        var actor = ActorFactory.CreatePlayerActor(profile);

        if(client.Account == null)
        {
            Log.Error($"[LoginHandler]: Failed to SpawnPlayer as the account for the client isn't set.");
            return;
        }

        var playerComp = new PlayerComponent()
        {
            Client = client,
            AccountName = client.Account.Username,
            Id = profile.Id,
            Name = profile.Name,
            CreatedAt = profile.CreatedAt,
            ModelType = profile.ModelType,
            FootprintRadius = profile.FootprintRadius,
            LineOfSight = profile.LineOfSight,
            AudibleRange = profile.AudibleRange,
            CurrentTileX = profile.CurrentTileX,
            CurrentTileZ = profile.CurrentTileZ,
            CurrentHealth = profile.CurrentHealth,
            MaxHealth = profile.MaxHealth,
            Strength = profile.Strength,
            Dexterity = profile.Dexterity,
            Constitution = profile.Constitution,
            Intelligence = profile.Intelligence,
            Wisdom = profile.Wisdom,
            Charisma = profile.Charisma,
            HealthRegenPerTick = profile.HealthRegenPerTick,
            HealthRegenIntervalSeconds = profile.HealthRegenIntervalSeconds,
            HealthRegenMultiplier = profile.HealthRegenMultiplier
        };

        
        actor.AddComponent(playerComp);

        AccountManager.GetAccountByUsername(client.Account.Username)?.Save();


        RealmTile? spawnTile = await GameDirector.RequestSpawn(
            actor, profile.CurrentTileX, profile.CurrentTileZ);

        if (spawnTile == null)
        {
            Log.Warn($"[LoginHandler]: No valid spawn tile for '{profile.Name}'. Moving to 0,0.");
            
            spawnTile = await GameDirector.RequestSpawn(actor, 0, 0);
        }

        if (spawnTile == null)
        {
            Log.Error($"[LoginHandler]: No valid spawn tile for '{profile.Name}' at 0,0. Disconnecting.");

            ClientManager.RemoveRealmClient(client);
            return;
        }

    }

    #endregion Spawn Player
}

/*
 *------------------------------------------------------------
 * (LoginHandler.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */