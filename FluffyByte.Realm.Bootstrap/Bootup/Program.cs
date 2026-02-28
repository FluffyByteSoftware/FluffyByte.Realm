using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Actors.Agents.AI;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Shared.CryptoTool;
using FluffyByte.Realm.Shared.Misc;
using FluffyByte.Realm.Tools.Disk;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class Program
{
    private static bool _serverRunning;

    public static void Main()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            if (_serverRunning) StopServer();
        };

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            if (_serverRunning) StopServer();
        };

        Console.Title = "Taerin's Whisper - Realm Server";

        DiskManager.Initialize();
        AccountManager.Initialize();

        while (true)
        {
            PrintMenu();
            var input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    ToggleServer();
                    break;
                case "2":
                    AccountMenu();
                    break;
                case "3":
                    Console.WriteLine($"Status panel not yet implemented.");
                    break;
                case "0":
                    if (_serverRunning)
                        StopServer();
                    return;
                default:
                    break;

            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== Taerin's Whisper ===");
        Console.WriteLine($"  1 - {(_serverRunning ? "Stop" : "Start")} Server");
        Console.WriteLine("  2 - Account Management");
        Console.WriteLine("  3 - System Status (NYI)");
        Console.WriteLine("  0 - Exit");
        Console.Write("> ");
    }

    #region Server 
    private static void ToggleServer()
    {
        if (_serverRunning)
            StopServer();

        else
            StartServer();        
    }

    private static void StartServer()
    {
        SystemOperator.InitializeSystem();
        SystemOperator.StartSystem();
        _serverRunning = true;
        Console.WriteLine("Server started successfully.");
    }

    private static void StopServer()
    {
        SystemOperator.ShutdownSystem();
        _serverRunning = false;
        Console.WriteLine("Server stopped successfully.");
    }
    #endregion Server

    #region Account Management
    private static void AccountMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- Account Management ---");
            Console.WriteLine(" 1 - Create Account");
            Console.WriteLine(" 2 - Delete Account");
            Console.WriteLine(" 3 - Reset Password");
            Console.WriteLine(" 0 - Back to Main Menu");
            Console.Write("> ");

            var input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    CreateAccount();
                    break;
                case "2":
                    DeleteAccount();
                    break;
                case "3":
                    ResetPassword();
                    break;
                case "0":
                    return;
                default: 
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        }
    }

    private static void DeleteAccount()
    {
        Console.Write("Username to delete: ");
        var username = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Cancelled.");
            return;
        }

        var account = AccountManager.GetAccountByUsername(username);

        if(account == null)
        {
            Console.WriteLine($"Account: '{username}' was not found.");
            return;
        }

        Console.WriteLine($"Are you sure you want to delete '{username}' yes/no?");
        var confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm != "yes")
        {
            Console.WriteLine("Cancelled. Must explicitly type yes.");
            return;
        }

        if(AccountManager.DeleteAccount(username))
            Console.WriteLine($"Account '{username}' deleted successfully.");
        else
            Console.WriteLine($"Failed to delete account '{username}'.");
    }

    private static void ResetPassword()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Cancelled.");
            return;
        }

        var account = AccountManager.GetAccountByUsername(username);

        if (account == null)
        {
            Console.WriteLine($"Account '{username}' not found.");
            return;
        }

        Console.Write("New Password: ");
        var password = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine($"Cancelled.");
            return;
        }

        account.PasswordHash = CryptoManager.EncryptPassword(password);
        account.Save();

        Console.WriteLine($"Password reset for '{username}' successfully.");
    }

    private static void CreateAccount()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine($"Username cannot be empty.");
            return;
        }

        Console.Write("Password: ");
        var password = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine($"Password cannot be empty.");
            return;
        }

        try
        {
            var encrypted = CryptoManager.EncryptPassword(password);
            var account = new RealmAccount(username, encrypted);
            account.Save();

            Console.WriteLine($"Account '{username}' created.");           
        }
        catch(ArgumentException ex)
        {
            Log.Error(ex);
        }
        
    }
    #endregion Account Management
}