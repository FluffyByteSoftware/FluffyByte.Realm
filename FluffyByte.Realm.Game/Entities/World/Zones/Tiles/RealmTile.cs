/*
 * (Tile.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@8:36:05 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Game.Brains.Assistants;
using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Events;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles.TileComponents;
using FluffyByte.Realm.Tools.Broadcasting;

namespace FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

/// <summary>
/// A RealmTile represents the most granular level of space within the game world.
/// It should translate to a 1x1 Godot unit, square.
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
    public DateTime? ColdSince { get; private set; }

    #endregion Identity

    #region Neighbors

    public RealmTile[] Neighbors { get; private set; } = [];

    public void SetNeighbors(RealmTile[] neighbors) => Neighbors = neighbors;

    #endregion Neighbors

    #region Occupants

    public GameObject? Agent { get; private set; }
    public List<GameObject> Items { get; } = [];

    public bool HasAgent => Agent != null;

    #endregion Occupants

    #region Components

    private readonly Dictionary<Type, TileComponent> _components = [];

    private readonly Dictionary<TickType, List<TileComponent>> _tickBuckets = new()
    {
        { TickType.Fast,   [] },
        { TickType.Normal, [] },
        { TickType.Slow,   [] }
    };

    #endregion Components

    #region Command Queue

    private readonly ConcurrentQueue<ITileCommand> _commandQueue = [];

    #endregion Command Queue

    #region Constructor

    public RealmTile(int x, int z, int globalX, int globalZ)
    {
        X       = x;
        Z       = z;
        GlobalX = globalX;
        GlobalZ = globalZ;
    }

    #endregion Constructor

    #region Lifecycle

    public void OnWarmLoad()
    {
        var old     = LoadState;
        var elapsed = ColdSince.HasValue
            ? DateTime.UtcNow - ColdSince.Value
            : TimeSpan.Zero;

        var missedFastTicks   = (long)(elapsed.TotalMilliseconds / Metronome.FastIntervalMs);
        var missedNormalTicks = (long)(elapsed.TotalMilliseconds / Metronome.NormalIntervalMs);
        var missedSlowTicks   = (long)(elapsed.TotalMilliseconds / Metronome.SlowIntervalMs);

        ColdSince = null;
        LoadState = RealmTileLoadState.Warm;

        foreach (var component in _components.Values)
            component.OnWarmLoad(elapsed, missedFastTicks, missedNormalTicks, missedSlowTicks);

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
        var old   = LoadState;
        LoadState = RealmTileLoadState.Cold;
        ColdSince = DateTime.UtcNow;

        foreach (var component in _components.Values)
            component.OnColdUnload();

        _commandQueue.Clear();

        PublishStateChange(old, LoadState);
    }

    private void PublishStateChange(RealmTileLoadState old, RealmTileLoadState next)
        => EventManager.Publish(new RealmTileLoadStateChangedEvent
        {
            Tile     = this,
            OldState = old,
            NewState = next
        });

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

    #region Item Management

    public void AddItem(GameObject item)
    {
        if (!Items.Contains(item))
            Items.Add(item);
    }

    public void RemoveItem(GameObject item)
        => Items.Remove(item);

    #endregion Item Management

    #region Enter / Exit Management

    public void OnTileEntered(GameObject enteringObject)
    {
        if (enteringObject.GetType() == typeof(IUniqueActor))
            TrySetAgent(enteringObject);

        EventManager.Publish(new RealmTileEnterTileEvent
        {
            Tile           = this,
            ObjectEntering = enteringObject
        });
    }

    public void OnTileExited(GameObject exitingObject)
    {
        if (exitingObject.GetType() == typeof(IUniqueActor))
            ClearAgent(exitingObject);

        EventManager.Publish(new RealmTileExitTileEvent
        {
            Tile          = this,
            ObjectLeaving = exitingObject
        });
    }

    #endregion Enter / Exit Management

    #region Component Management

    public void AddComponent<T>(T component) where T : TileComponent
    {
        var type = typeof(T);

        if (_components.ContainsKey(type))
            throw new InvalidOperationException(
                $"[RealmTile ({X},{Z})]: Already has component {type.Name}.");

        component.Owner = this;
        _components[type] = component;
        
        if(component.TickType != TickType.None)
            _tickBuckets[component.TickType].Add(component);
    }

    public void RemoveComponent<T>() where T : TileComponent
    {
        var type = typeof(T);

        if (!_components.TryGetValue(type, out var component))
            return;

        _tickBuckets[component.TickType].Remove(component);
        _components.Remove(type);
        component.OnColdUnload();
    }

    public T? GetComponent<T>() where T : TileComponent
    {
        _components.TryGetValue(typeof(T), out var component);
        return component as T;
    }

    public bool HasComponent<T>() where T : TileComponent
        => _components.ContainsKey(typeof(T));

    #endregion Component Management

    #region Command Queue

    public void EnqueueCommand(ITileCommand command)
        => _commandQueue.Enqueue(command);

    private void DrainCommands()
    {
        while (_commandQueue.TryDequeue(out var command))
            command.Execute(this);
    }

    #endregion Command Queue

    #region Ticks

    public void ActiveTick(TickType tickType)
    {
        if (tickType == TickType.Fast)
            DrainCommands();

        Agent?.Tick(tickType);

        foreach (var item in Items)
            item.Tick(tickType);

        foreach (var component in _tickBuckets[tickType])
            component.ActiveTick();
    }

    public void WarmTick(TickType tickType)
    {
        foreach (var component in _tickBuckets[tickType])
            component.WarmTick();
    }

    #endregion Ticks

    #region Diagnostics

    public int ComponentCount => _components.Count;
    public int ItemCount      => Items.Count;

    public override string ToString()
        => $"RealmTile ({X},{Z}) Global({GlobalX},{GlobalZ}) State={LoadState} Agent={Agent?.Name ?? "none"} Items={Items.Count}";

    #endregion Diagnostics
}
/*
 *------------------------------------------------------------
 * (Tile.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */