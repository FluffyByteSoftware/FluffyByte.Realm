using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;
using FluffyByte.Realm.Game.Entities.World;
using FluffyByte.Realm.Game.Entities.World.Zones;
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

    Log.Info("[StressTest]: Spawning 50 unique actors...");
    
    for (var i = 0; i < 50; i++)
    {
        var x = random.Next(0, RealmWorld.ZoneCountX * RealmZone.Width);
        var z = random.Next(0, RealmWorld.ZoneCountZ * RealmZone.Height);

        var zone = world.TryGetZone(x, z);
        if (zone is null) { i--; continue; }

        if (!zone.IsLoaded)
        {
            zone.OnLoad();
            zone.WireBorderNeighbors();
        }

        var tile = world.GetTile(x, z);
        if (tile.HasAgent) { i--; continue; }

        var actor = ActorFactory.CreateUniqueActor($"unique_{i}");
        GameDirector.RegisterActor(actor, tile);
    }
    for (var i = 0; i < 100000; i++)
    {
        var go = new GameObject($"Test{i}");
        go.AddComponent(new TransformComponent());

        var zone = world.TryGetZone(1, 1);

        if (zone is null)
        {
            Log.Warn($"[StressTest]: Failed to spawn gameobject {i} zone is null");
            break;
        }

        if (!zone.IsLoaded)
        {
            zone.OnLoad();
            zone.WireBorderNeighbors();
        }
        go.GetTransform()?.Tile = world.GetTile(1, 1);
        Log.Debug($"Spawned gameobject {i}");
    }
    
    Log.Info("[StressTest]: Unique actors spawned. Waiting for world to warm...");
    Thread.Sleep(2000);

    Log.Info("[StressTest]: Filling every hot tile with an actor...");

    var filled = 0;
    foreach (var tile in GameDirector.HotTiles)
    {
        if (tile.HasAgent) continue;

        ActorFactory.CreateLivingActor(
            $"npc_{filled}",
            health: 100, mana: 100,
            strength: 10, dexterity: 10, constitution: 10,
            intelligence: 10, wisdom: 10, charisma: 10,
            startingTile: tile);

        filled++;
    }

    Log.Info($"[StressTest]: Filled {filled} hot tiles with actors.");

    // Spawn one tracked NPC with 1HP to watch regen
    var trackedTile = GameDirector.HotTiles.FirstOrDefault(t => !t.HasAgent);
    GameObject? trackedNpc = null;

    if (trackedTile != null)
    {
        trackedNpc = ActorFactory.CreateLivingActor(
            "tracked_npc",
            health: 1, mana: 10,
            strength: 10, dexterity: 10, constitution: 10,
            intelligence: 10, wisdom: 10, charisma: 10,
            startingTile: trackedTile);

        trackedNpc.GetComponent<Health>()?.Current = 1;
        trackedNpc.GetComponent<Health>()?.Max = 1000;
        
        Log.Info("[StressTest]: Spawned tracked_npc with 1HP.");
    }

    var monitor = new Thread(() =>
    {
        var end = DateTime.UtcNow.AddSeconds(60);

        while (DateTime.UtcNow < end)
        {
            Thread.Sleep(1000);

            var hp    = trackedNpc?.GetComponent<Health>()?.Current ?? -1;
            var maxHp = trackedNpc?.GetComponent<Health>()?.Max ?? -1;

            Log.Debug($"[StressTest]: {GameDirector.Status()} " +
                      $"Fast={GameDirector.FastTickCount} " +
                      $"Normal={GameDirector.NormalTickCount} " +
                      $"Slow={GameDirector.SlowTickCount} " +
                      $"| tracked_npc HP={hp}/{maxHp}");
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