/*
 * (GameDirector.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@10:59:34 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Brains.Assistants;
using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;
using FluffyByte.Realm.Game.Entities.World;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains;

public static class GameDirector
{
    #region Assistants

    private static WorldComposer _composer = null!;
    private static Metronome _metronome = null!;
    private static ActorRegistrar _actorReg = null!;
    private static PlayerRegistrar _playerRegistrar = null!;
    #endregion Assistants
    
    #region Config

    public static GameDirectorConfig Config { get; private set; } = null!;
    private static GameDirectorEditor _editor = null!;

    #endregion Config
    
    #region World

    public static RealmWorld World { get; private set; } = null!;
    
    #endregion World
    
    #region Initialization

    private static bool _isInitialized;
    
    public static void Initialize()
    {

        if (_isInitialized) return;
        
        _editor = new GameDirectorEditor();
        Config = _editor.Load();
        World = new RealmWorld();
        _composer = new WorldComposer(World);
        _metronome = new Metronome();
        _actorReg = new ActorRegistrar(_composer);
        _playerRegistrar = new PlayerRegistrar();
        
        _isInitialized = true;
        
        EventManager.Subscribe<SystemStartupEvent>(OnSystemStartup);
        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
    }
    #endregion Initialization
    
    #region Lifecycle Events

    private static void OnSystemStartup(SystemStartupEvent e)
    {
        if (!_isInitialized) return;
        
        World.OnLoad();
        _metronome.Start();
        
        Log.Debug($"[GameDirector]: Initialized.");
    }

    private static void OnSystemShutdown(SystemShutdownEvent e)
    {
        if(!_isInitialized) return;

        _metronome.Stop();
        _composer.OnUnload();
        World.OnUnload();
        _editor.Save(Config);
        
        _editor = null!;
        Config = null!;
        World = null!;
        _composer = null!;
        _metronome = null!;
        _actorReg = null!;

        _playerRegistrar.SaveAll();
        
        _isInitialized = false;
        
        EventManager.Unsubscribe<SystemStartupEvent>(OnSystemStartup);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
    }
    #endregion Lifecycle Events
    
    #region Actor Registration
    public static void RegisterUniqueActor(GameObject actor)
        => _actorReg.RegisterUnique(actor);

    public static void UnregisterUniqueActor(GameObject actor)
        => _actorReg.UnregisterUnique(actor);

    public static void OnUniqueActorMoved(GameObject actor)
        => _actorReg.OnUniqueActorMoved(actor);
    
    #endregion Actor Registration
    
    #region Player Registration

    public static void DeletePlayerProfile(Guid id) => _playerRegistrar.DeleteProfile(id);
    
    public static void SavePlayerProfiles() => _playerRegistrar.SaveAll();
    
    public static void CreatePlayerProfile(string name) => _playerRegistrar.CreateProfile(name);
    
    public static void SavePlayerProfile(PlayerProfile profile) => _playerRegistrar.Save(profile);
    
    public static PlayerProfile? GetPlayerProfile(string name) => _playerRegistrar.GetByName(name);
    public static PlayerProfile? GetPlayerProfile(Guid id) => _playerRegistrar.GetById(id);
    
    public static bool PlayerProfileExists(string name) => _playerRegistrar.ProfileExists(name);
    
    #endregion Player Registration
    
    #region Object Spawn
    
    public static RealmTile? WakeBeforeSpawn(int globalX, int globalZ)
    {
        var zone = World.TryGetZone(globalX, globalZ);
        if (zone is null) return null;

        if (!zone.IsLoaded)
        {
            zone.OnLoad();
            zone.WireBorderNeighbors();
        }

        return World.TryGetTile(globalX, globalZ);
    }
    
    public static void SpawnGameObject(GameObject objectToSpawn, RealmTile tile)
    {
        WakeBeforeSpawn(tile.GlobalX, tile.GlobalZ);

        var transform = objectToSpawn.GetComponent<TransformComponent>();

        if (transform == null)
        {
            Log.Warn($"[GameDirector]: Spawn request for {objectToSpawn.Name} does not have a TransformComponent. " +
                     $"Cannot spawn.");
            return;
        }

        if (objectToSpawn.HasComponent<ActorComponent>())
        {
            if (tile.HasAgent)
            {
                Log.Warn($"[GameDirector]: Cannot spawn {objectToSpawn.Name} on tile " +
                         $"{tile.GlobalX},{tile.GlobalZ} because it already has an agent.");
                Log.Warn($"[GameDirector]: {objectToSpawn.Name} will not be spawned.");
                return;
            }
        }
        
        transform.Tile = tile;

        tile.OnTileEntered(objectToSpawn);
        objectToSpawn.OnSpawn();

        if (objectToSpawn.UniqueObjectType != UniqueObjectType.None)
            _actorReg.RegisterUnique(objectToSpawn);
        
        
    }
    
    #endregion Object Spawn
    
    #region Config Management

    public static void UpdateConfig(Action<GameDirectorConfig> mutation)
    {
        mutation(Config);
        _editor.Save(Config);
    }
    #endregion Config Management
    
    #region Tick Handlers

    
    public static void ActiveTick(TickType tickType)
    {
        _actorReg.UpdateIfNeeded();
        _composer.ActiveTick(tickType);
    }

    public static void WarmTick(TickType tickType)
        => _composer.WarmTick(tickType);
    
    public static long FastTickCount => _metronome.FastTickCount;
    public static long NormalTickCount => _metronome.NormalTickCount;
    public static long SlowTickCount => _metronome.SlowTickCount;
    
    #endregion Tick Handlers
    
    #region Diagnostics

    public static int ActorCount => _actorReg.UniqueActorCount;
    public static int HotCount => _composer.HotCount;
    public static int WarmCount => _composer.WarmCount;

    public static string Status()
        => $"GameDirector Agents={ActorCount} Hot={HotCount} Warm={WarmCount}";
    
    
    public static RealmTile? GetRandomHotTile(Random random)
    {
        var hotTiles = _composer.HotTiles;
        var warmTiles = _composer.WarmTiles;
        var total = hotTiles.Count + warmTiles.Count;

        if (total == 0) return null;

        var index = random.Next(0, total);

        return index < hotTiles.Count
            ? hotTiles.ElementAt(index)
            : warmTiles.ElementAt(index - hotTiles.Count);

    }

    public static IReadOnlySet<RealmTile> HotTiles => _composer.HotTiles;

    #endregion Diagnostics

}


/*
 *------------------------------------------------------------
 * (GameDirector.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */