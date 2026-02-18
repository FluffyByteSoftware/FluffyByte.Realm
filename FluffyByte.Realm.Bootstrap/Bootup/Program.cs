using System.Text;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Shared.CryptoTool;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class Program
{
    public static void Main()
    {
        Startup();

        Console.ReadLine();

        Shutdown();
    }

    private static void CreateSeliris()
    {
        var pass = "password123";
        
        var encryptedPassword = CryptoManager.EncryptPassword(pass);

        var account = new RealmAccount("seliris", encryptedPassword);
        
        account.Save();

        Log.Info($"Created account '{account.Username}' with password '{pass}'.");
    }
    
    private static void Startup()
    {
        SystemOperator.InitializeSystem();

        Task.Delay(100);
    }

    private static void Shutdown()
    {
        SystemOperator.ShutdownSystem();
        
    }
}