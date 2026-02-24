/*
 * (WorldComposer.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 22, 2026@9:41:37 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.World;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains.Assistants;

public class WorldComposer(RealmWorld world)
{
    #region Active Tile Sets

    private readonly HashSet<RealmTile> _hotTiles = [];
    private readonly HashSet<RealmTile> _warmTiles = [];

    public IReadOnlySet<RealmTile> HotTiles => _hotTiles;
    public IReadOnlySet<RealmTile> WarmTiles => _warmTiles;

    #endregion Active Tile Sets
    
    #region Lifecycle

    public void OnUnload()
    {
        foreach(var tile in _hotTiles)
            tile.OnColdUnload();
        
        foreach(var tile in _warmTiles)
            tile.OnColdUnload();

        _hotTiles.Clear();
        _warmTiles.Clear();
    }
    #endregion Lifecycle
    
    #region Tick Fan-out

    public void ActiveTick(TickType tickType)
    {
        foreach (var tile in _hotTiles)
        {
            tile.ActiveTick(tickType);
        }
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
    
    public void Refresh(IReadOnlyDictionary<IUniqueActor, RealmTile> actorTiles)
{
    _desiredHot.Clear();
    _desiredWarm.Clear();
    _hotToWarm.Clear();
    _warmToHot.Clear();
    _hotToCold.Clear();
    _warmToCold.Clear();
    _newHot.Clear();
    _newWarm.Clear();

    foreach (var (_, actorTile) in actorTiles)
    {
        CollectTilesForAgent(actorTile, _desiredHot, _desiredWarm);
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
        RealmTile actorTile,
        HashSet<RealmTile> desiredHot,
        HashSet<RealmTile> desiredWarm)
    {
        var cx = actorTile.GlobalX;
        var cz = actorTile.GlobalZ;
        var hotR = GameDirector.Config.HotRadius;
        var warmR = GameDirector.Config.WarmRadius;
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

    public override string ToString() => $"WorldComposer Hot={HotCount} Warm={WarmCount}";

    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (WorldComposer.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */