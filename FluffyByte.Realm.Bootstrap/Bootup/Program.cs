using System.Diagnostics;
using System.Text;
using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Game.Brains.Assistants;
using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World;
using FluffyByte.Realm.Game.Entities.World.Zones;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Shared.CryptoTool;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class Program
{
    public static void Main()
    {
        Startup();

        //CreateSeliris();

        StressTest();
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
        
        SystemOperator.StartSystem();
    }

    private static void Shutdown()
    {
        SystemOperator.ShutdownSystem();
        
    }
    
    private static void StressTest()
    {
        var random = new Random();
        var world = GameDirector.World;
        
        Log.Info($"[StressTest]: Spawning 50 unique actors...");

        for (var i = 0; i < 50; i++)
        {
            var x = random.Next(0, RealmWorld.ZoneCountX * RealmZone.Width);
            var z = random.Next(0, RealmWorld.ZoneCountZ * RealmZone.Height);
            
            // Lazy load zone before accessing
            var zone = world.TryGetZone(x, z);
            if (zone is null)
            {
                i--;
                continue;
            }

            if (!zone.IsLoaded)
            {
                zone.OnLoad();
                zone.WireBorderNeighbors();
            }

            var tile = world.GetTile(x, z);

            if (tile.HasAgent)
            {
                i--;
                continue;
            }

            var actor = ActorFactory.CreateUniqueActor(
                $"StressTest_{i}",
                10, 10, 10, 10, 10, 10, 10, 10,
                tile, PrimitiveModelType.Capsule, 1);

            GameDirector.RegisterActor(actor, tile);
        }

        Log.Info($"[StressTest]: All actors spawned. Monitoring tick times...");

        var monitor = new Thread(() =>
        {
            var end = DateTime.UtcNow.AddSeconds(30);

            while (DateTime.UtcNow < end)
            {
                Thread.Sleep(1000);

                Log.Debug($"[StressTest]: {GameDirector.Status()} " +
                          $"Fast={GameDirector.FastTickCount}" +
                          $"Normal={GameDirector.NormalTickCount}" +
                          $"Slow={GameDirector.SlowTickCount}");
            }

            Log.Info("[StressTest]: Stopping monitoring thread.");
        })
        {
            IsBackground = true,
            Name = "StressMonitor"
        };

        monitor.Start();
    }
}