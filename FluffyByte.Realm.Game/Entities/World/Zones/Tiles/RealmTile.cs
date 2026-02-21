/*
 * (Tile.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@8:36:05 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Game.Entities.Events;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles.TileComponents;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

/// <summary>
/// 
/// </summary>
public class RealmTile
{
    #region Identity

    public int X { get; }
    public int Z { get; }
    public int GlobalX { get; }
    public int GlobalZ { get; }
    public RealmZone? Zone { get; set; }
    public RealmTileLoadState LoadState { get; private set; } = RealmTileLoadState.Cold;

    #endregion Identity
    
    #region Neighbors

    public RealmTile[] Neighbors { get; private set; } = [];

    public void SEtNeighbors(RealmTile[] neighbors) => Neighbors = neighbors;

    #endregion
    
    #region Occupants

    public GameObject? Agent { get; private set; }
    public List<GameObject> Items { get; } = [];

    public bool HasAgent => Agent != null;
    #endregion Occupants
    
    #region Components

    private readonly Dictionary<Type, TileComponent> _components = [];

    private readonly Dictionary<TickType, List<TileComponent>> _tickBuckets = new()
    {
        { TickType.Fast, [] },
        { TickType.Normal, [] },
        { TickType.Slow, [] }
    };

    #endregion Components
    
    #region Command Queue
    private readonly ConcurrentQueue<ITileCommand> _commandQueue = [];
    #endregion Command Queue
    
    #region Constructor

    public RealmTile(int x, int z, int globalX, int globalZ)
    {
        X = x;
        Z = z;
        GlobalX = globalX;
        GlobalZ = globalZ;
    }
    #endregion Constructor
    
    #region Lifecycle

    public void OnWarmLoad()
    {
        var old = LoadState;
        LoadState = RealmTileLoadState.Warm;

        foreach (var component in _components.Values)
            component.OnWarmLoad();

        PublishStateChange(old, LoadState);
    }

    public void OnHotLoad()
    {
        var old = LoadState;
        LoadState = RealmTileLoadState.Hot;

        foreach (var component in _components.Values)
            component.OnHotLoad();

        PublishStateChange(old, LoadState);
    }

    public void OnWarmUnload()
    {
        var old = LoadState;
        LoadState = RealmTileLoadState.Warm;

        foreach (var component in _components.Values)
            component.OnWarmUnload();

        PublishStateChange(old, LoadState);
    }

    public void OnColdUnload()
    {
        var old = LoadState;
        LoadState = RealmTileLoadState.Cold;

        foreach (var component in _components.Values)
            component.OnColdUnload();

        Agent = null;
        
        _commandQueue.Clear();

        PublishStateChange(old, LoadState);
    }
    #endregion Lifecycle
    
    #region Agent Management

    public bool TrySetAgent(GameObject agent)
    {
        if (HasAgent)
            return false;

        Agent = agent;
        
        return true;
    }

    public void ClearAgent(GameObject agent)
    {
        if (Agent != agent)
            return;

        Agent = null;
    }
    #endregion Agent Management
    
    #region Enter/Exit Management

    public void OnTileEntered(RealmTileEnterTileEvent e)
    {
    }
    #endregion Enter/Exit Management
}
/*
 *------------------------------------------------------------
 * (Tile.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */