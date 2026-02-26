/*
 * (LoginHandler.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@12:29:20 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Accounting;
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

    private static void OnAuthSuccess(OnAuthenticationSuccessEvent e)
    {
        var account = AccountManager.GetAccountByUsername(e.AccountName);

        if (account == null)
        {
            Log.Warn($"Account '{e.AccountName}' not found after login success.");
            e.Client.Disconnect();
            return;
        }

        var packet = new CharacterListPacket()
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

        e.Client.SendPacket(PacketType.CharacterList, packet);
    }
}

/*
 *------------------------------------------------------------
 * (LoginHandler.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */