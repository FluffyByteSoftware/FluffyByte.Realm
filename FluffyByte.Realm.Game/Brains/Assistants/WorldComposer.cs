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
            tile.ActiveTick(tickType);
    }

    public void WarmTick(TickType tickType)
    {
        foreach (var tile in _warmTiles)
            tile.WarmTick(tickType);
    }
    #endregion Tick Fan-out
    
    #region Tile Refresh

    public void Refresh(IReadOnlyDictionary<IUniqueActor, RealmTile> actorTiles)
    {
        var desiredHot = new HashSet<RealmTile>();
        var desiredWarm = new HashSet<RealmTile>();

        foreach (var (_, actorTile) in actorTiles)
        {
            CollectTilesForAgent(actorTile, desiredHot, desiredWarm);
        }
        
        // Hot always wins
        desiredWarm.ExceptWith(desiredHot);
        
        // Currently hot tiles that need to drop to warm
        var hotToWarm = new HashSet<RealmTile>(_hotTiles);
        hotToWarm.IntersectWith(desiredWarm);

        var warmToHot = new HashSet<RealmTile>(_warmTiles);
        warmToHot.IntersectWith(desiredHot);

        var hotToCold = new HashSet<RealmTile>(_hotTiles);
        hotToCold.ExceptWith(desiredHot);
        hotToCold.ExceptWith(desiredWarm);

        var warmToCold = new HashSet<RealmTile>(_warmTiles);
        warmToCold.ExceptWith(desiredHot);
        warmToCold.ExceptWith(desiredWarm);

        var newHot = new HashSet<RealmTile>(desiredHot);
        newHot.ExceptWith(_hotTiles);
        newHot.ExceptWith(_warmTiles);

        var newWarm = new HashSet<RealmTile>(desiredWarm);
        newWarm.ExceptWith(_warmTiles);
        newWarm.ExceptWith(_hotTiles);
        
        // Transitions

        foreach (var tile in hotToCold)
        {
            _hotTiles.Remove(tile);
            tile.OnColdUnload();
        }

        foreach (var tile in warmToCold)
        {
            _warmTiles.Remove(tile);
            tile.OnColdUnload();
        }

        foreach (var tile in hotToWarm)
        {
            _hotTiles.Remove(tile);
            _warmTiles.Add(tile);
            
            tile.OnWarmUnload();
        }

        foreach (var tile in newWarm)
        {
            _warmTiles.Add(tile);
            tile.OnWarmLoad();
        }
        
        foreach (var tile in warmToHot)
        {
            _warmTiles.Remove(tile);
            _hotTiles.Add(tile);
            tile.OnHotLoad();
        }

        foreach (var tile in newHot)
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
        {
            for (var dz = -warmR; dz <= warmR; dz++)
            {
                // Chebyshev check — skip anything outside the warm square
                if (Math.Max(Math.Abs(dx), Math.Abs(dz)) > warmR)
                    continue;

                var tile = world.TryGetTile(cx + dx, cz + dz);
                if (tile is null)
                    continue;

                // Euclidean check — inside hot circle or outer warm ring
                var distSq = dx * dx + dz * dz;

                if (distSq <= hotR2)
                    desiredHot.Add(tile);
                else
                    desiredWarm.Add(tile);
            }
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