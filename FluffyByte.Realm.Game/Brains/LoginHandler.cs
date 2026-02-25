/*
 * (LoginHandler.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@12:29:20 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;

namespace FluffyByte.Realm.Game.Brains;

public static class LoginHandler
{
    #region Life Cycle
    
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
        if (!_isInitialized)
            return;
        
        _isInitialized = false;

        EventManager.Unsubscribe<OnAuthenticationSuccessEvent>(OnAuthSuccess);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
    }
    #endregion Life Cycle
    
    #region AuthFlow

    private static void OnAuthSuccess(OnAuthenticationSuccessEvent e)
    {
        var account = AccountManager.GetAccountByUsername(e.AccountName);
    }
    #endregion AuthFlow
}

/*
 *------------------------------------------------------------
 * (LoginHandler.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */