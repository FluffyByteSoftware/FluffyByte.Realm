/*
 * (ActorRegistrar.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@9:32:51 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Brains.Assistants;

/// <summary>
/// ActorRegistrar is the GameDirector's assistant responsible for tracking
/// the tile positions of all IUniqueActor instances in the world.
///
/// It owns the actor-to-tile dictionary, the read/write lock, and the dirty flag.
/// When an actor's state changes, it marks itself dirty. On UpdateIfNeeded(), it snapshots
/// the current state and triggers a WorldComposer refresh, keeping tile warming
/// accurate without blocking the Metronome thread.
/// </summary>
public class ActorRegistrar(WorldComposer composer)
{
    #region State

    private readonly ReaderWriterLockSlim _lock = new();
    private readonly Dictionary<IUniqueActor, RealmTile> _uniqueActorTiles = [];
    private volatile bool _isDirty;

    #endregion State

    #region Registration

    public void RegisterUnique(IUniqueActor actor, RealmTile startingTile)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_uniqueActorTiles.TryAdd(actor, startingTile))
                return;
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        _isDirty = true;
    }

    public void UnregisterUnique(IUniqueActor actor)
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

    public void OnUniqueActorMoved(IUniqueActor actor, RealmTile newTile)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_uniqueActorTiles.TryGetValue(actor, out var currentTile) || currentTile == newTile)
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

        Dictionary<IUniqueActor, RealmTile> snapshot;

        _lock.EnterReadLock();
        try
        {
            snapshot = new Dictionary<IUniqueActor, RealmTile>(_uniqueActorTiles);
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