/*
 * (RealmZone.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 19, 2026@11:40:38 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Game.Entities.Primitives.Events;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Heartbeats;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Entities.Primitives.Zones;


public class RealmZone : IGameObjectOwner
{
    // ── Identity ──────────────────────────────────────────────────────────────
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }

    // ── Clock Names ───────────────────────────────────────────────────────────
    private string FastClockName   => $"{Name}.Fast";
    private string NormalClockName => $"{Name}.Normal";
    private string SlowClockName   => $"{Name}.Slow";

    private const int FastIntervalMs   = 100;
    private const int NormalIntervalMs = 500;
    private const int SlowIntervalMs   = 2000;

    private Clock? _fastClock;
    private Clock? _normalClock;
    private Clock? _slowClock;

    // ── State ─────────────────────────────────────────────────────────────────
    public bool IsActive { get; private set; } = true;

    // ── Neighbors ─────────────────────────────────────────────────────────────
    /// <summary>
    /// Adjacent zones that should be woken when an IZoneActivator enters this zone.
    /// Populate after all zones are constructed via AddNeighbor().
    /// </summary>
    private readonly List<RealmZone> _neighbors = [];

    // ── GameObject Storage ────────────────────────────────────────────────────
    private readonly ConcurrentDictionary<Guid, GameObject> _gameObjects = new();

    // ── Constructor ───────────────────────────────────────────────────────────
    public RealmZone(string name)
    {
        Name = name;
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Call when this zone is loaded into the world.
    /// Registers and starts its three tick clocks.
    /// </summary>
    public void OnLoad()
    {
        _fastClock   = ClockManager.RegisterClock(FastClockName,   FastIntervalMs);
        _normalClock = ClockManager.RegisterClock(NormalClockName, NormalIntervalMs);
        _slowClock   = ClockManager.RegisterClock(SlowClockName,   SlowIntervalMs);

        _fastClock.OnTick   += OnFastTick;
        _normalClock.OnTick += OnNormalTick;
        _slowClock.OnTick   += OnSlowTick;

        ClockManager.StartClock(_fastClock);
        ClockManager.StartClock(_normalClock);
        ClockManager.StartClock(_slowClock);

        Log.Info($"[RealmZone]: '{Name}' loaded with {_gameObjects.Count} object(s).");
    }

    /// <summary>
    /// Call when this zone is unloaded from the world.
    /// Stops clocks, unsubscribes, destroys all GameObjects.
    /// </summary>
    public void OnUnload()
    {
        if (_fastClock != null)
        {
            _fastClock.OnTick -= OnFastTick;
            ClockManager.StopClock(_fastClock);
            ClockManager.UnregisterClock(FastClockName);
            _fastClock = null;
        }

        if (_normalClock != null)
        {
            _normalClock.OnTick -= OnNormalTick;
            ClockManager.StopClock(_normalClock);
            ClockManager.UnregisterClock(NormalClockName);
            _normalClock = null;
        }

        if (_slowClock != null)
        {
            _slowClock.OnTick -= OnSlowTick;
            ClockManager.StopClock(_slowClock);
            ClockManager.UnregisterClock(SlowClockName);
            _slowClock = null;
        }

        foreach (var obj in _gameObjects.Values)
            obj.OnDestroy();

        _gameObjects.Clear();

        Log.Info($"[RealmZone]: '{Name}' unloaded.");
    }

    // ── IGameObjectOwner ──────────────────────────────────────────────────────

    public void AddGameObject(GameObject obj)
    {
        if (_gameObjects.TryAdd(obj.Id, obj))
        {
            Log.Debug($"[RealmZone]: '{Name}' — added {obj.Name}.");
            ReevaluateActiveState();
        }
        else
        {
            Log.Warn($"[RealmZone]: '{Name}' — {obj.Name} already present, skipping.");
        }
    }

    public void RemoveGameObject(GameObject obj)
    {
        if (_gameObjects.TryRemove(obj.Id, out _))
        {
            Log.Debug($"[RealmZone]: '{Name}' — removed {obj.Name}.");
            ReevaluateActiveState();
        }
    }

    public bool ContainsGameObject(Guid id) => _gameObjects.ContainsKey(id);

    public GameObject? GetGameObject(Guid id)
    {
        _gameObjects.TryGetValue(id, out var obj);
        return obj;
    }

    public IReadOnlyCollection<GameObject> GetAllGameObjects()
        => _gameObjects.Values.ToList().AsReadOnly();

    // ── Neighbors ─────────────────────────────────────────────────────────────

    public void AddNeighbor(RealmZone zone)
    {
        if (!_neighbors.Contains(zone))
            _neighbors.Add(zone);
    }

    public void RemoveNeighbor(RealmZone zone)
    {
        _neighbors.Remove(zone);
    }

    // ── Active State ──────────────────────────────────────────────────────────

    /// <summary>
    /// Re-evaluates whether this zone should be active based on IZoneActivator presence.
    /// Wakes self and neighbors when an activator is present, sleeps when there aren't any.
    /// </summary>
    private void ReevaluateActiveState()
    {
        var hasActivator = _gameObjects.Values.Any(
            obj => obj.GetType().IsAssignableTo(typeof(IZoneActivator)));

        if (hasActivator)
        {
            Wake();
            foreach (var neighbor in _neighbors)
                neighbor.Wake();
        }
        else
        {
            Sleep();
        }
    }

    /// <summary>
    /// Stops this zone's clocks — GameObjects stop ticking.
    /// </summary>
    public void Sleep()
    {
        if (!IsActive) return;

        IsActive = false;

        if (_fastClock != null)   ClockManager.StopClock(_fastClock);
        if (_normalClock != null) ClockManager.StopClock(_normalClock);
        if (_slowClock != null)   ClockManager.StopClock(_slowClock);

        Log.Info($"[RealmZone]: '{Name}' going to sleep.");
    }

    /// <summary>
    /// Starts this zone's clocks — GameObjects resume ticking.
    /// </summary>
    public void Wake()
    {
        if (IsActive) return;

        IsActive = true;

        if (_fastClock != null)   ClockManager.StartClock(_fastClock);
        if (_normalClock != null) ClockManager.StartClock(_normalClock);
        if (_slowClock != null)   ClockManager.StartClock(_slowClock);

        Log.Info($"[RealmZone]: '{Name}' waking up.");
    }

    // ── Tick Handlers ─────────────────────────────────────────────────────────

    private void OnFastTick()
    {
        if (!IsActive) return;

        foreach (var obj in _gameObjects.Values)
            obj.Tick(TickType.Fast);

        EventManager.Publish(new ZoneTickFastEvent { Zone = this });
    }

    private void OnNormalTick()
    {
        if (!IsActive) return;

        foreach (var obj in _gameObjects.Values)
            obj.Tick(TickType.Normal);

        EventManager.Publish(new ZoneTickNormalEvent { Zone = this });
    }

    private void OnSlowTick()
    {
        if (!IsActive) return;

        foreach (var obj in _gameObjects.Values)
            obj.Tick(TickType.Slow);

        EventManager.Publish(new ZoneTickSlowEvent { Zone = this });
    }

    // ── Diagnostics ───────────────────────────────────────────────────────────
    public int GameObjectCount => _gameObjects.Count;
    public int NeighborCount   => _neighbors.Count;

    public override string ToString()
        => $"RealmZone '{Name}' [{Id}] — {_gameObjects.Count} object(s), Active={IsActive}";
}

/*
 *------------------------------------------------------------
 * (RealmZone.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */