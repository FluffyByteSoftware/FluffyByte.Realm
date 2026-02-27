using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Actors.Agents.AI;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Shared.CryptoTool;
using FluffyByte.Realm.Shared.Misc;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class Program
{
    public static void Main()
    {
        Startup();

        //CreateSeliris();

        StressTest();
        
        //CreatePlayerSeliris();
        
        Console.ReadLine();

        Shutdown();
    }

    private static void CreatePlayerSeliris()
    {
        var selirisProfile = new PlayerProfile()
        {
            LineOfSight = 350,
            AudibleRange = 200,
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
            PreviousTileX = 0,
            PreviousTileZ = 0,
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
        var random = new Random(42);
        const int actorCount = 30;
        const int sightRange = 50;
        const int audibleRange = 30;

        Log.Info($"[StressTest]:  Spawning {actorCount} elite actors with SightRange={sightRange}...");

        for (var i = 0; i < actorCount; i++)
        {
            var template = new ActorTemplate()
            {
                Name = $"StressActor_{i:D3}",
                CurrentTileX = 0,
                CurrentTileZ = 0,
                PreviousTileX = 0,
                PreviousTileZ = 0,
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
                LineOfSight = sightRange,
                AudibleRange = audibleRange,
                ModelType = PrimitiveModelType.Capsule,
                ComplexModelType = ComplexModelType.DefaultAndrogynous,
                FootprintRadius = 1,
                Id = Guid.NewGuid(),
            };
            
            var actor = ActorFactory.CreateEliteActor(template);

            var rand = new Random(0);
            
            var wandering = new WanderingAi(100);
            actor.AddComponent(wandering);
       
            Log.Debug($"[StressTest]: Created {actor.Name}");
            
            var spawnX = random.Next(2000, 190000);
            var spawnZ = random.Next(2000, 190000);
            
            GameDirector.RequestSpawn(actor, spawnX, spawnZ);
            Log.Info($"[StressTest]: Queued {actor.Name} at ({spawnX}, {spawnZ})");
        }

        var monitor = new Thread(() =>
        {
            var end = DateTime.UtcNow.AddSeconds(60);
            var tick = 0;

            while (DateTime.UtcNow < end)
            {
                Thread.Sleep(2000);
                tick++;

                var proc = System.Diagnostics.Process.GetCurrentProcess();
                var memMb = proc.WorkingSet64 / (1024 * 1024);

                Log.Debug($"[StressTest][{tick:D3}]: {GameDirector.Status()} " +
                          $"Fast={GameDirector.FastTickCount} " +
                          $"Normal={GameDirector.NormalTickCount} " +
                          $"Slow={GameDirector.SlowTickCount} " +
                          $"RAM={memMb}MB");
            }

            Log.Info($"[StressTest]: Complete.");
        })
        {
            IsBackground = true,
            Name = "StressMonitor"
        };

        monitor.Start();
    }
}