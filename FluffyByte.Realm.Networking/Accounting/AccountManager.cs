/*
 * (AccountManager.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 15, 2026@1:38:56 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Heartbeats;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Accounting;

public static class AccountManager
{
    private static bool _isInitialized;

    private const string AccountsPath = @"accounts/";
    private const string AccountExtension = ".account";
    private const string ClockName = "account-updater";
    
    private static readonly List<RealmAccount> Accounts = [];
    private static readonly HashSet<string> KnownFiles = [];
    
    #region Life Cycle

    public static void Initialize()
    {
        if (_isInitialized)
            return;

        if (!Directory.Exists(AccountsPath))
        {
            Directory.CreateDirectory(AccountsPath);
            Log.Info($"[AccountManager]: Created accounts directory at '{AccountsPath}'.");
        }

        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
        EventManager.Subscribe<TickEvent>(Tick);
        
        ClockManager.RegisterClock(ClockName, 3000);
        ClockManager.StartClock(ClockName);

        _isInitialized = true;

        AccountUpdate();
        
        Log.Info($"[AccountManager]: Initialized with {Accounts.Count} accounts.");
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized)
            return;
        
        _isInitialized = false;
        
        SaveAccounts();

        ClockManager.StopClock(ClockName);
        ClockManager.UnregisterClock(ClockName);

        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
    }

    private static void Tick(TickEvent e)
    {
        if (e.ClockName != ClockName)
            return;
        
        AccountUpdate();
    }
    #endregion Life Cycle
    
    #region Account Loading
    private static void AccountUpdate()
    {
        var files = Directory.GetFiles(AccountsPath, $"*{AccountExtension}");

        foreach (var file in files)
        {
            if (KnownFiles.Contains(file))
                continue;

            Log.Info($"[AccountManager]: Detected new account file: {file}");
            LoadAccountFile(file);
        }
    }

    private static void LoadAccountFile(string filePath)
    {
        if (KnownFiles.Contains(filePath))
            return;

        var account = RealmAccount.Load(filePath);
        
        if (account == null) 
            return;

        Accounts.Add(account);
        KnownFiles.Add(filePath);
    }

    private static void SaveAccounts()
    {
        foreach (var account in Accounts)
        {
            account.Save();
        }

        Log.Info($"[AccountManager]: Saved {Accounts.Count} accounts.");
    }
    #endregion Account Loading
    
    #region Lookups

    public static RealmAccount? GetAccountByUsername(string username)
    {
        return Accounts.FirstOrDefault(a =>
            string.Equals(a.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public static bool AccountExists(string username)
    {
        return Accounts.Any(a =>
            string.Equals(a.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public static int AccountCount => Accounts.Count;

    #endregion Lookups
}

/*
 *------------------------------------------------------------
 * (AccountManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */