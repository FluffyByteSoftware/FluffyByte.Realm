using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Game.Entities.Complex;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Shared.CryptoTool;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class Program
{
    public static void Main()
    {
        Startup();

        //CreateSeliris();

        //StressTest();
        
        CreatePlayerSeliris();
        
        Console.ReadLine();

        Shutdown();
    }

    private static void CreatePlayerSeliris()
    {
        var selirisProfile = new PlayerProfile()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Name = "seliris",
            CurrentHealth = 100,
            MaxHealth = 100,
            HealthRegenPerTick = 1,
            HealthRegenIntervalSeconds = 5,
            HealthRegenMultiplier = 1,
            Strength = 10,
            Dexterity = 10,
            Constitution = 10,
            Intelligence = 10,
            Wisdom = 10,
            Charisma = 10,
            CurrentTileX = 0,
            CurrentTileZ = 0,
            ModelType = PrimitiveModelType.Capsule,
            ComplexModelType = ComplexModelType.DefaultMasculine,
            FootprintRadius = 1
        };

        GameDirector.SavePlayerProfile(selirisProfile);

        GameDirector.SavePlayerProfiles();
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

        SystemOperator.StartSystem();
    }

    private static void Shutdown()
    {
        SystemOperator.ShutdownSystem();

    }

    private static void StressTest()
    {


        
        var monitor = new Thread(() =>
        {
            var end = DateTime.UtcNow.AddSeconds(60);

            while (DateTime.UtcNow < end)
            {
                Thread.Sleep(1000);

                Log.Debug($"[StressTest]: {GameDirector.Status()} " +
                          $"Fast={GameDirector.FastTickCount} " +
                          $"Normal={GameDirector.NormalTickCount} " +
                          $"Slow={GameDirector.SlowTickCount} ");
            }

            Log.Info("[StressTest]: Complete.");
        })
        {
            IsBackground = true,
            Name         = "StressMonitor"
        };

        monitor.Start();
    }
}