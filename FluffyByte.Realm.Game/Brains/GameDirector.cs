/*
 * (GameDirector.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@10:59:34 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Game.Brains.Assistants;
using FluffyByte.Realm.Game.Brains.Helpers;
using FluffyByte.Realm.Game.Entities.Actors;
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

    #region Queues

    private static readonly ConcurrentQueue<SpawnRequest> SpawnQueue = new();
    private static readonly ConcurrentQueue<MoveRequest> MoveQueue = new();

    #endregion Queues

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
        
        _playerRegistrar.LoadAll();

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

        Log.Debug("[GameDirector]: Initialized.");
    }

    private static void OnSystemShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized) return;

        _metronome.Stop();
        _composer.OnUnload();
        World.OnUnload();
        _editor.Save(Config);

        _playerRegistrar.SaveAll();

        _editor = null!;
        Config = null!;
        World = null!;
        _composer = null!;
        _metronome = null!;
        _actorReg = null!;
        _playerRegistrar = null!;

        _isInitialized = false;

        EventManager.Unsubscribe<SystemStartupEvent>(OnSystemStartup);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
    }

    #endregion Lifecycle Events

    #region Actor Registration

    /// <summary>
    /// Registers the specified actor as a unique entity within the game.
    /// </summary>
    /// <remarks>Use this method to add new actors to the game while ensuring that each actor is only
    /// registered once. Attempting to register the same actor multiple times will have no effect.</remarks>
    /// <param name="actor">The GameObject representing the actor to register. Cannot be null.</param>
    public static void RegisterUniqueActor(GameObject actor)
        => _actorReg.RegisterUnique(actor);

    /// <summary>
    /// Unregisters the specified actor from the unique actor registry.
    /// </summary>
    /// <remarks>Call this method when an actor is no longer needed or is being destroyed to ensure it is
    /// properly removed from the registry. Failing to unregister actors may result in memory leaks or unintended
    /// behavior due to lingering references.</remarks>
    /// <param name="actor">The GameObject representing the actor to be unregistered. This parameter must not be null.</param>
    public static void UnregisterUniqueActor(GameObject actor)
        => _actorReg.UnregisterUnique(actor);

    /// <summary>
    /// Notifies the system that a unique actor has moved, triggering any necessary updates related to the actor's new
    /// position.
    /// </summary>
    /// <remarks>Call this method when an actor's position changes in the game world to ensure that all
    /// relevant systems are updated accordingly.</remarks>
    /// <param name="actor">The GameObject representing the actor that has moved. This parameter must not be null.</param>
    public static void OnUniqueActorMoved(GameObject actor)
        => _actorReg.OnUniqueActorMoved(actor);

    #endregion Actor Registration

    #region Player Registration
    /// <summary>
    /// Deletes the player profile associated with the specified identifier.
    /// </summary>
    /// <remarks>This method will throw an exception if the specified profile does not exist or if the
    /// deletion fails due to internal errors.</remarks>
    /// <param name="id">The unique identifier of the player profile to be deleted. This value must be a valid GUID representing an
    /// existing profile.</param>
    public static void DeletePlayerProfile(Guid id)
        => _playerRegistrar.DeleteProfile(id);

    /// <summary>
    /// Saves all player profiles to persistent storage.
    /// </summary>
    /// <remarks>This method invokes the underlying player registrar to perform the save operation. Ensure
    /// that the player profiles are properly initialized before calling this method.</remarks>
    public static void SavePlayerProfiles()
        => _playerRegistrar.SaveAll();

    /// <summary>
    /// Registers the specified player profile with the system, enabling the player to access personalized settings and
    /// data.
    /// </summary>
    /// <remarks>Ensure that the profile is fully populated with all required data before calling this method.
    /// An exception may be thrown if the profile is invalid.</remarks>
    /// <param name="profile">The player profile to register. This parameter must not be null and should contain valid player information.</param>
    public static void RegisterPlayerProfile(PlayerProfile profile) 
        => _playerRegistrar.Register(profile);
    
    /// <summary>
    /// Saves the specified player profile to the registrar.
    /// </summary>
    /// <remarks>Ensure that the profile is fully populated with the necessary information before calling this
    /// method.</remarks>
    /// <param name="profile">The player profile to be saved. This object must not be null and should contain valid player data.</param>
    public static void SavePlayerProfile(PlayerProfile profile)
        => _playerRegistrar.Save(profile);

    /// <summary>
    /// Retrieves the player profile associated with the specified player name.
    /// </summary>
    /// <remarks>If the specified name does not correspond to any registered player, the method returns null.
    /// Ensure that the name provided is valid and corresponds to an existing player profile.</remarks>
    /// <param name="name">The name of the player whose profile is to be retrieved. This parameter cannot be null or empty.</param>
    /// <returns>A PlayerProfile object representing the player's profile if found; otherwise, null.</returns>
    public static PlayerProfile? GetPlayerProfile(string name)
        => _playerRegistrar.GetByName(name);

    /// <summary>
    /// Retrieves the player profile associated with the specified unique identifier.
    /// </summary>
    /// <remarks>This method interacts with the player registrar to fetch the profile. Ensure that the
    /// provided ID corresponds to an existing player to avoid receiving a null result.</remarks>
    /// <param name="id">The unique identifier of the player whose profile is to be retrieved. This parameter cannot be an empty GUID.</param>
    /// <returns>A PlayerProfile object representing the player's profile if found; otherwise, null if no profile exists for the
    /// given identifier.</returns>
    public static PlayerProfile? GetPlayerProfile(Guid id)
        => _playerRegistrar.GetById(id);

    /// <summary>
    /// Determines whether a player profile with the specified name exists.
    /// </summary>
    /// <remarks>This method checks the underlying player registrar for the existence of the profile. Ensure
    /// that the name provided is valid and corresponds to an existing profile.</remarks>
    /// <param name="name">The name of the player whose profile existence is to be checked. This parameter cannot be null or empty.</param>
    /// <returns>true if a profile with the specified name exists; otherwise, false.</returns>
    public static bool PlayerProfileExists(string name)
        => _playerRegistrar.ProfileExists(name);

    #endregion Player Registration

    #region Spawn System

    /// <summary>
    /// Enqueues a request to spawn the specified game object at the given global coordinates within a defined search
    /// radius.
    /// </summary>
    /// <remarks>The spawn request is processed asynchronously. Ensure that the actor is valid and that the
    /// specified coordinates are appropriate for spawning. The method does not guarantee immediate or successful
    /// spawning.</remarks>
    /// <param name="actor">The game object to spawn. This parameter cannot be null.</param>
    /// <param name="globalX">The global X coordinate at which to attempt spawning the game object. Must be within the valid game world
    /// boundaries.</param>
    /// <param name="globalZ">The global Z coordinate at which to attempt spawning the game object. Must be within the valid game world
    /// boundaries.</param>
    /// <param name="searchRadius">The radius, in world units, within which to search for a suitable spawn location. Defaults to 5 units.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the spawned realm tile if
    /// successful; otherwise, null.</returns>
    public static Task<RealmTile?> RequestSpawn(GameObject actor,
        int globalX,
        int globalZ,
        int searchRadius = 5)
    {
        var request = new SpawnRequest
        {
            GameObjectToSpawn = actor,
            GlobalX = globalX,
            GlobalZ = globalZ,
            SearchRadius = searchRadius
        };

        SpawnQueue.Enqueue(request);

        return request.Completion.Task;
    }

    /// <summary>
    /// Retrieves a list of game objects that are currently observing the specified realm tile.
    /// </summary>
    /// <remarks>This method is static and relies on the internal composer to fetch the observers. Ensure that
    /// the realm tile is valid before calling this method.</remarks>
    /// <param name="tile">The realm tile for which to retrieve the observers. This parameter cannot be null.</param>
    /// <returns>A list of game objects that are observing the specified realm tile. The list will be empty if no observers are
    /// present.</returns>
    public static List<GameObject> GetObservers(RealmTile tile) => _composer.GetObservers(tile);

    private static void DrainSpawnQueue()
    {
        var retryCount = SpawnQueue.Count;

        for (var i = 0; i < retryCount; i++)
        {
            if (!SpawnQueue.TryDequeue(out var request))
                break;
            
            var tile = FindValidSpawnTile(request.GlobalX, request.GlobalZ, request.SearchRadius);
            
            if (tile == null)
            {
                SpawnQueue.Enqueue(request);
                continue;
            }

            SpawnGameObject(request.GameObjectToSpawn, tile);
            request.Completion.SetResult(tile);
        }
    }

    private static void SpawnGameObject(GameObject objectToSpawn, RealmTile tile)
    {
        Log.Debug($"[GameDirector]: SpawnGameObject: Entering for {objectToSpawn.Name} " +
                  $"hasTransform={objectToSpawn.HasComponent<TransformComponent>()} " +
                  $"hasActor={objectToSpawn.HasComponent<ActorComponent>()} " +
                  $"unique={objectToSpawn.UniqueObjectType}");
        
        WakeBeforeSpawn(tile.GlobalX, tile.GlobalZ);

        var transform = objectToSpawn.GetTransform();

        if (transform == null)
        {
            Log.Warn($"[GameDirector]: Spawn request for {objectToSpawn.Name} does not have a " +
                     $"TransformComponent. Cannot spawn.");
            return;
        }

        if (objectToSpawn.HasComponent<ActorComponent>())
        {
            if (tile.IsGroundBlocked)
            {
                if (tile.HasAgent)
                {
                    Log.Warn($"[GameDirector]: Cannot spawn {objectToSpawn.Name} on tile (" +
                             $"{tile.GlobalX},{tile.GlobalZ}) because it already has an agent.");
                    
                }
                else
                {
                    Log.Warn($"[GameDirector]: Cannot spawn {objectToSpawn.Name} on tile " +
                             $"{tile.GlobalX},{tile.GlobalZ} because its ground is obstructed.");
                }

                Log.Warn($"[GameDirector]: Ground is blocked for {objectToSpawn.Name}.  Not sure why." +
                         $"");
                return;
            }
        }

        transform.Tile = tile;
        tile.OnTileEntered(objectToSpawn);
        objectToSpawn.OnSpawn();

        if (objectToSpawn.UniqueObjectType != UniqueObjectType.None)
        {
            _actorReg.RegisterUnique(objectToSpawn);
            Log.Debug($"[Spawn]: Registered {objectToSpawn.Name} unique={objectToSpawn.UniqueObjectType} " +
                      $"regCount={_actorReg.UniqueActorCount}");
        }
        
    }

    private static RealmTile? FindValidSpawnTile(int globalX, int globalZ, int searchRadius = 5)
    {
        var preferred = WakeBeforeSpawn(globalX, globalZ);

        Log.Debug($"[FindSpawn]: ({globalX},{globalZ}) tile={preferred != null} " +
                  $"blocked={preferred?.IsGroundBlocked} pass={preferred?.Passability} " +
                  $"agent={preferred?.HasAgent}");
        
        if (preferred is { IsGroundBlocked: false })
            return preferred;

        for (var r = 1; r <= searchRadius; r++)
        {
            for (var dx = -r; dx <= r; dx++)
            for (var dz = -r; dz <= r; dz++)
            {
                if (Math.Abs(dx) != r && Math.Abs(dz) != r)
                    continue;

                var tile = WakeBeforeSpawn(globalX + dx, globalZ + dz);

                if (tile is { IsGroundBlocked: false })
                    return tile;
            }
        }

        return null;
    }

    private static RealmTile? WakeBeforeSpawn(int globalX, int globalZ)
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


    #endregion Spawn System

    #region Move System

    /// <summary>
    /// Requests that the specified actor move to the given global coordinates asynchronously.
    /// </summary>
    /// <remarks>The move request is queued and processed asynchronously. The returned task completes when the
    /// move is executed. If the move is not possible, the task result will be null.</remarks>
    /// <param name="actor">The game object representing the actor to move. Cannot be null.</param>
    /// <param name="globalX">The target global X coordinate to which the actor should move.</param>
    /// <param name="globalZ">The target global Z coordinate to which the actor should move.</param>
    /// <returns>A task that represents the asynchronous move operation. The task result is the realm tile the actor will occupy
    /// upon completion, or null if the move could not be performed.</returns>
    public static Task<RealmTile?> RequestMove(GameObject actor, int globalX, int globalZ)
    {
        var request = new MoveRequest
        {
            GameObject = actor,
            TargetGlobalX = globalX,
            TargetGlobalZ = globalZ
        };

        MoveQueue.Enqueue(request);

        return request.Completion.Task;
    }

    private static void DrainMoveQueue()
    {
        while (MoveQueue.TryDequeue(out var request))
            ProcessMove(request);
    }

    private static void ProcessMove(MoveRequest request)
    {
        var actor = request.GameObject;
        var transform = actor.GetTransform();

        if (transform?.Tile == null)
        {
            Log.Warn($"[GameDirector]: Move request for {actor.Name} failed. No transform or tile.");
            request.Completion.SetResult(null);
            return;
        }

        var targetTile = WakeBeforeSpawn(request.TargetGlobalX, request.TargetGlobalZ);

        if (targetTile == null)
        {
            Log.Warn($"[GameDirector]: Move request for {actor.Name} failed. " +
                     $"Target tile ({request.TargetGlobalX},{request.TargetGlobalZ}) is invalid.");
            request.Completion.SetResult(null);
            return;
        }

        if (targetTile.IsGroundBlocked)
        {
            request.Completion.SetResult(null);
            return;
        }

        var oldTile = transform.Tile;

        oldTile.OnTileExited(actor);
        transform.Tile = targetTile;
        targetTile.OnTileEntered(actor);

        if (actor.UniqueObjectType != UniqueObjectType.None)
            _actorReg.OnUniqueActorMoved(actor);

        request.Completion.SetResult(targetTile);
    }

    #endregion Move System

    #region Config Management

    /// <summary>
    /// Applies a specified modification to the current game director configuration and immediately persists the
    /// changes.
    /// </summary>
    /// <remarks>Use this method to update the game director configuration at runtime. The configuration is
    /// saved automatically after the mutation is applied, ensuring that changes are not lost.</remarks>
    /// <param name="mutation">An action that defines the changes to apply to the current game director configuration. Cannot be null.</param>
    public static void UpdateConfig(Action<GameDirectorConfig> mutation)
    {
        mutation(Config);
        _editor.Save(Config);
    }

    #endregion Config Management

    #region Tick Handlers

    /// <summary>
    /// Processes a game tick of the specified type, updating the system state and managing queued actions as needed.
    /// </summary>
    /// <remarks>Call this method regularly to ensure timely updates to the game state. The behavior of this
    /// method may vary depending on the provided tick type, which can affect how queued actions are handled.</remarks>
    /// <param name="tickType">The type of tick to process. If set to TickType.Fast, the method drains the spawn and move queues before
    /// performing updates.</param>
    public static void ActiveTick(TickType tickType)
    {
        if (tickType == TickType.Fast)
        {
            DrainSpawnQueue();
            DrainMoveQueue();
        }

        _actorReg.UpdateIfNeeded();
        _composer.ActiveTick(tickType);
    }

    /// <summary>
    /// Processes a warm tick of the specified type, triggering any associated actions.
    /// </summary>
    /// <param name="tickType">The type of tick to process, which influences the behavior of the warm tick operation.</param>
    public static void WarmTick(TickType tickType)
        => _composer.WarmTick(tickType);

    /// <summary>
    /// Gets the current tick count in milliseconds, providing a high-resolution timer for performance measurements.
    /// </summary>
    /// <remarks>This property retrieves the tick count from the underlying metronome, which is optimized for
    /// speed. It is suitable for scenarios where precise timing is required, such as benchmarking or measuring elapsed
    /// time in high-frequency applications.</remarks>
    public static long FastTickCount => _metronome.FastTickCount;
    /// <summary>
    /// Gets the current tick count of the metronome, representing the number of ticks since the application started.
    /// </summary>
    /// <remarks>This property provides a way to access the tick count without directly interacting with the
    /// metronome instance. It is useful for timing operations or measuring intervals in applications that rely on the
    /// metronome's timing.</remarks>
    public static long NormalTickCount => _metronome.NormalTickCount;
    /// <summary>
    /// Gets the current slow tick count from the metronome.
    /// </summary>
    /// <remarks>This property provides access to the slow tick count, which may be used for timing or
    /// synchronization purposes in applications that require precise control over timing events.</remarks>
    public static long SlowTickCount => _metronome.SlowTickCount;

    #endregion Tick Handlers

    #region Diagnostics

    /// <summary>
    /// Gets the total number of unique actors currently registered in the system.
    /// </summary>
    /// <remarks>This property is useful for monitoring and diagnostics, providing a real-time count of actors
    /// as they are added or removed from the registry.</remarks>
    public static int ActorCount => _actorReg.UniqueActorCount;
    /// <summary>
    /// Gets the current number of hot items managed by the composer.
    /// </summary>
    /// <remarks>This property provides a read-only view of the hot item count, which may be useful for
    /// monitoring, diagnostics, or logging purposes. The value reflects the state of the underlying composer at the
    /// time of access.</remarks>
    public static int HotCount => _composer.HotCount;
    /// <summary>
    /// Gets the number of items that are currently maintained in a warm (preloaded or cached) state by the composer.
    /// </summary>
    /// <remarks>The warm count reflects the total number of items that are preloaded or cached for faster
    /// access. This property is read-only and is typically used to monitor or optimize resource management
    /// strategies.</remarks>
    public static int WarmCount => _composer.WarmCount;


    /// <summary>
    /// Gets a summary of the current game director status, including the number of active agents and their temperature
    /// states.
    /// </summary>
    /// <returns>A string that describes the current status of the game director, formatted to include the total agent count and
    /// the counts of agents in hot and warm states.</returns>
    public static string Status()
        => $"GameDirector Agents={ActorCount} Hot={HotCount} Warm={WarmCount}";

    /// <summary>
    /// Selects and returns a random hot or warm tile from the available tiles.
    /// </summary>
    /// <remarks>If there are no hot or warm tiles, the method returns null. The selection is uniformly
    /// distributed across all available hot and warm tiles.</remarks>
    /// <param name="random">The random number generator used to select a tile.</param>
    /// <returns>A randomly selected hot tile if any are available; otherwise, a randomly selected warm tile. Returns null if no
    /// hot or warm tiles are available.</returns>
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