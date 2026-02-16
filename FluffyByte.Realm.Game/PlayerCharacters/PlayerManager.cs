/*
 * (PlayerManager.cs)
 *------------------------------------------------------------
 * Created - Monday, February 16, 2026@8:09:42 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.PlayerCharacters;

public static class PlayerManager
{
    private static bool _isInitialized; 
    
    public static void Initialize()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        Log.Debug($"[PlayerManager]: Initialized.");
        
        EventManager.Subscribe<OnAuthenticationSuccessEvent>(OnAuthenticationSuccess);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        
        if(!_isInitialized) return;
        
        _isInitialized = false;
        
        EventManager.Unsubscribe<OnAuthenticationSuccessEvent>(OnAuthenticationSuccess);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
    }

    private static void OnAuthenticationSuccess(OnAuthenticationSuccessEvent e)
    {
        Log.Debug($"{e.Client.Name}: Reached game point.");
    }
}

/*
 *------------------------------------------------------------
 * (PlayerManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */