/*
 * (ActorRegistrar.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@9:32:51 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Brains.Assistants;

/// <summary>
/// Manages the registration and tracking of unique actors (game objects) and their corresponding positions
/// (tiles) within the game world. Provides functionality to add, remove, update, and retrieve information
/// about unique actors and their locations. Synchronizes the state changes with the provided
/// <see cref="WorldComposer"/> instance.
/// </summary>
/// <remarks>
/// This class ensures thread-safe operations using read/write locks for managing the registration
/// data. Additionally, it tracks if the state has changed and updates the associated composer
/// only when necessary to reduce overhead.
/// </remarks>
/// <example>
/// This class does not provide usage examples.
/// </example>
public class ActorRegistrar(WorldComposer composer)
{
    #region State

    private readonly ReaderWriterLockSlim _lock = new();
    private readonly Dictionary<GameObject, RealmTile> _uniqueActorTiles = [];
    private volatile bool _isDirty;

    #endregion State

    #region Registration

    /// <summary>
    /// Registers a unique actor (game object) within the game world and associates it with a specified starting tile.
    /// If the actor is already registered, the operation is ignored. Updates the state to reflect the change.
    /// </summary>
    /// <param name="actor">The game object representing the unique actor to register.</param>
    public void RegisterUnique(GameObject actor)
    {
        _lock.EnterWriteLock();
        try
        {
            var startingTile = actor.GetTransform()?.Tile;
            
            if (startingTile == null)
                return;
            
            if (!_uniqueActorTiles.TryAdd(actor, startingTile))
                return;
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        _isDirty = true;
    }

    /// <summary>
    /// Unregisters a previously registered unique actor from the game world.
    /// If the actor is not currently registered, the operation is ignored.
    /// Marks the state as changed to trigger any necessary updates.
    /// </summary>
    /// <param name="actor">The game object representing the unique actor to unregister.</param>
    public void UnregisterUnique(GameObject actor)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_uniqueActorTiles.Remove(actor))
                return;
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        _isDirty = true;
    }

    public void OnUniqueActorMoved(GameObject actor)
    {
        _lock.EnterWriteLock();
        
        try
        {
            if (!_uniqueActorTiles.TryGetValue(actor, out var currentTile))
                return;
            
            var newTile = actor.GetTransform()?.Tile;

            if (newTile == null || newTile == currentTile)
                return;
            
            _uniqueActorTiles[actor] = newTile;
            
            _isDirty = true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    #endregion Registration

    #region Composer Sync

    public void UpdateIfNeeded()
    {
        if (!_isDirty) return;

        Dictionary<GameObject, RealmTile> snapshot;

        _lock.EnterReadLock();
        try
        {
            snapshot = new Dictionary<GameObject, RealmTile>(_uniqueActorTiles);
        }
        finally
        {
            _lock.ExitReadLock();
        }

        _isDirty = false;
        composer.Refresh(snapshot);
    }

    #endregion Composer Sync

    #region Diagnostics

    public int UniqueActorCount
    {
        get
        {
            _lock.EnterReadLock();
            try { return _uniqueActorTiles.Count; }
            finally { _lock.ExitReadLock(); }
        }
    }

    public override string ToString() => $"ActorRegistrar UniqueActors={UniqueActorCount}";

    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (ActorRegistrar.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */