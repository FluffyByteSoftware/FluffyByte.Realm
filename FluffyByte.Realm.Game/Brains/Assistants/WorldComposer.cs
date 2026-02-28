/*
 * (WorldComposer.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 22, 2026@9:41:37 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Brains.Helpers;
using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Actors.Events;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
using FluffyByte.Realm.Tools.Broadcasting;

namespace FluffyByte.Realm.Game.Brains.Assistants;
public class WorldComposer(RealmWorld world)
{
    #region Active Tile Sets

    private readonly HashSet<RealmTile> _hotTiles = [];
    private readonly HashSet<RealmTile> _warmTiles = [];

    public IReadOnlySet<RealmTile> HotTiles => _hotTiles;
    public IReadOnlySet<RealmTile> WarmTiles => _warmTiles;

    #endregion Active Tile Sets

    #region Per-Actor Cache

    private readonly Dictionary<GameObject, RealmTile> _lastKnownTile = [];
    private readonly Dictionary<GameObject, HashSet<RealmTile>> _cachedHotPerActor = [];
    private readonly Dictionary<GameObject, HashSet<RealmTile>> _cachedWarmPerActor = [];

    #endregion Per-Actor Cache

    #region Lifecycle

    public void OnUnload()
    {
        foreach (var tile in _hotTiles)
            tile.OnColdUnload();

        foreach (var tile in _warmTiles)
            tile.OnColdUnload();

        _hotTiles.Clear();
        _warmTiles.Clear();
        _lastKnownTile.Clear();
        _cachedHotPerActor.Clear();
        _cachedWarmPerActor.Clear();
    }

    #endregion Lifecycle

    #region Tick Fan-out

    public void ActiveTick(TickType tickType)
    {
        foreach (var tile in _hotTiles)
            tile.ActiveTick(tickType);
    }

    public void WarmTick(TickType tickType)
    {
        foreach (var tile in _warmTiles)
            tile.WarmTick(tickType);
    }

    #endregion Tick Fan-out

    #region Tile Refresh

    private readonly HashSet<RealmTile> _desiredHot = [];
    private readonly HashSet<RealmTile> _desiredWarm = [];
    private readonly HashSet<RealmTile> _hotToWarm = [];
    private readonly HashSet<RealmTile> _warmToHot = [];
    private readonly HashSet<RealmTile> _hotToCold = [];
    private readonly HashSet<RealmTile> _warmToCold = [];
    private readonly HashSet<RealmTile> _newHot = [];
    private readonly HashSet<RealmTile> _newWarm = [];

    /// <summary>
    /// Retrieves a list of game objects that are currently observing the specified realm tile.
    /// </summary>
    /// <remarks>This method checks both hot and warm observer sets to determine which game objects are
    /// currently observing the specified tile.</remarks>
    /// <param name="tile">The realm tile for which to retrieve the observing game objects. This parameter cannot be null.</param>
    /// <returns>A list of game objects that are observing the specified realm tile. The list may be empty if no observers are
    /// found.</returns>
    public List<GameObject> GetObservers(RealmTile tile)
    {
        var observers = new List<GameObject>();

        foreach(var (actor, hotSet) in _cachedHotPerActor)
        {
            if (hotSet.Contains(tile))
            {
                observers.Add(actor);
                continue;
            }

            if(_cachedWarmPerActor.TryGetValue(actor, out var warmSet) && warmSet.Contains(tile))
            {
                observers.Add(actor);
            }
        }

        return observers;
    }

    /// <summary>
    /// Updates the internal state of tracked tiles and actors based on the current positions of all actors.
    /// </summary>
    /// <remarks>This method recalculates which tiles should be in hot, warm, or cold states according to
    /// actor movement. It removes data for actors that are no longer present and manages transitions between tile
    /// states, triggering appropriate load and unload events as needed. Call this method whenever actor positions
    /// change to ensure tile states remain consistent.</remarks>
    /// <param name="actorTiles">A read-only dictionary that maps each actor to its current tile. Represents the latest known positions of all
    /// active actors.</param>
    public void Refresh(IReadOnlyDictionary<GameObject, RealmTile> actorTiles)
    {
        _desiredHot.Clear();
        _desiredWarm.Clear();
        _hotToWarm.Clear();
        _warmToHot.Clear();
        _hotToCold.Clear();
        _warmToCold.Clear();
        _newHot.Clear();
        _newWarm.Clear();

        // Remove caches for actors no longer registered
        var staleActors = _lastKnownTile.Keys
            .Where(a => !actorTiles.ContainsKey(a))
            .ToList();

        foreach (var stale in staleActors)
        {
            _lastKnownTile.Remove(stale);
            _cachedHotPerActor.Remove(stale);
            _cachedWarmPerActor.Remove(stale);
        }

        // Collect tiles per actor — recalculate only if moved or new
        foreach (var (actor, actorTile) in actorTiles)
        {
            var hasMoved = !_lastKnownTile.TryGetValue(actor, out var lastTile) || lastTile != actorTile;

            if (hasMoved)
            {
                var previousHot = _cachedHotPerActor.GetValueOrDefault(actor);
                var previousWarm = _cachedWarmPerActor.GetValueOrDefault(actor);

                var actorHot = new HashSet<RealmTile>();
                var actorWarm = new HashSet<RealmTile>();

                CollectTilesForAgent(actor, actorTile, actorHot, actorWarm);

                _cachedHotPerActor[actor] = actorHot;
                _cachedWarmPerActor[actor] = actorWarm;

                var isFirst = !_lastKnownTile.ContainsKey(actor);

                var isNew = !_lastKnownTile.ContainsKey(actor);

                _lastKnownTile[actor] = actorTile;

                var tileEvent = new ActorTilesChangedEvent
                {
                    Actor = actor,
                    HotTiles = actorHot,
                    WarmTiles = actorWarm,
                    PreviousHotTiles = previousHot ?? [],
                    PreviousWarmTiles = previousWarm ?? [],
                    IsFirstRefresh = isFirst
                };

                EventManager.Publish(tileEvent);

                if (isNew)
                {
                    EventManager.Publish(new ActorTilesReadyEvent
                    {
                        Actor = actor,
                        HotTiles = actorHot,
                        WarmTiles = actorWarm
                    });
                }
            }

            // Union cached sets into desired
            _desiredHot.UnionWith(_cachedHotPerActor[actor]);
            _desiredWarm.UnionWith(_cachedWarmPerActor[actor]);
        }

        // Hot always wins
        _desiredWarm.ExceptWith(_desiredHot);

        _hotToWarm.UnionWith(_hotTiles);
        _hotToWarm.IntersectWith(_desiredWarm);

        _warmToHot.UnionWith(_warmTiles);
        _warmToHot.IntersectWith(_desiredHot);

        _hotToCold.UnionWith(_hotTiles);
        _hotToCold.ExceptWith(_desiredHot);
        _hotToCold.ExceptWith(_desiredWarm);

        _warmToCold.UnionWith(_warmTiles);
        _warmToCold.ExceptWith(_desiredHot);
        _warmToCold.ExceptWith(_desiredWarm);

        _newHot.UnionWith(_desiredHot);
        _newHot.ExceptWith(_hotTiles);
        _newHot.ExceptWith(_warmTiles);

        _newWarm.UnionWith(_desiredWarm);
        _newWarm.ExceptWith(_warmTiles);
        _newWarm.ExceptWith(_hotTiles);

        // Transitions

        foreach (var tile in _hotToCold)
        {
            _hotTiles.Remove(tile);
            tile.OnColdUnload();
        }

        foreach (var tile in _warmToCold)
        {
            _warmTiles.Remove(tile);
            tile.OnColdUnload();
        }

        foreach (var tile in _hotToWarm)
        {
            _hotTiles.Remove(tile);
            _warmTiles.Add(tile);
            tile.OnWarmUnload();
        }

        foreach (var tile in _newWarm)
        {
            _warmTiles.Add(tile);
            tile.OnWarmLoad();
        }

        foreach (var tile in _warmToHot)
        {
            _warmTiles.Remove(tile);
            _hotTiles.Add(tile);
            tile.OnHotLoad();
        }

        foreach (var tile in _newHot)
        {
            _hotTiles.Add(tile);
            tile.OnWarmLoad();
            tile.OnHotLoad();
        }
    }

    #endregion Tile Refresh

    #region Radius Math

    private void CollectTilesForAgent(
        GameObject actor,
        RealmTile actorTile,
        HashSet<RealmTile> desiredHot,
        HashSet<RealmTile> desiredWarm)
    {
        var cx = actorTile.GlobalX;
        var cz = actorTile.GlobalZ;
        var los = actor.GetComponent<LineOfSight>();
        var hotR = los?.SightRange ?? GameDirector.Config.HotRadius;
        var warmR = (int)Math.Round(hotR * GameDirector.Config.WarmRadiusMultiplier);

        var hotR2 = hotR * hotR;

        for (var dx = -warmR; dx <= warmR; dx++)
        for (var dz = -warmR; dz <= warmR; dz++)
        {
            if (Math.Max(Math.Abs(dx), Math.Abs(dz)) > warmR)
                continue;

            var globalX = cx + dx;
            var globalZ = cz + dz;

            // Lazy zone load
            var zone = world.TryGetZone(globalX, globalZ);
            if (zone is null) continue;

            if (!zone.IsLoaded)
            {
                zone.OnLoad();
                zone.WireBorderNeighbors();
            }

            var tile = world.TryGetTile(globalX, globalZ);
            if (tile is null) continue;

            var distSq = dx * dx + dz * dz;

            if (distSq <= hotR2)
                desiredHot.Add(tile);
            else
                desiredWarm.Add(tile);
        }
    }

    #endregion Radius Math

    #region Diagnostics

    public int HotCount => _hotTiles.Count;
    public int WarmCount => _warmTiles.Count;
    public int CachedActorCount => _lastKnownTile.Count;

    public override string ToString() =>
        $"WorldComposer Hot={HotCount} Warm={WarmCount} CachedActors={CachedActorCount}";

    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (WorldComposer.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */