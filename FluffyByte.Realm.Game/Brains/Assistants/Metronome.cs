/*
 * (Metronome.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@10:58:14 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Diagnostics;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains.Assistants;

/// <summary>
/// The Metronome is the GameDirector's assistant responsible for the fixed-timestep tick loop.
/// It runs on a dedicated background thread and drives GameDirector tick fan-out
/// at three configurable frequencies:
///
///   Fast   — high-frequency logic (command drain, movement, combat)
///   Normal — mid-frequency logic (AI decisions, status effects)
///   Slow   — low-frequency logic (ecology, resource regeneration, world events)
///
/// Uses an accumulator pattern to prevent tick bleed — each interval fires as many
/// times as it has fully elapsed, with the remainder carrying forward.
///
/// Exposes static interval values so RealmTile.OnWarmLoad() can compute missed ticks
/// from ColdSince without needing a direct reference to the Metronome.
/// </summary>
public class Metronome : IDisposable
{
    #region Static Interval Shortcuts

    /// <summary>
    /// Current Fast interval in ms. Synced from config on Start().
    /// Used by RealmTile for cold-to-warm missed tick calculations.
    /// </summary>
    public static int FastIntervalMs { get; private set; } = 20;

    /// <summary>Current Normal interval in ms. Synced from config on Start().</summary>
    public static int NormalIntervalMs { get; private set; } = 60;

    /// <summary>Current Slow interval in ms. Synced from config on Start().</summary>
    public static int SlowIntervalMs { get; private set; } = 100;

    #endregion Static Interval Shortcuts

    #region State

    private Thread? _thread;
    private CancellationTokenSource? _cts;
    private volatile bool _running;

    #endregion State

    #region Tick Accumulators

    private double _fastAccumulatorMs;
    private double _normalAccumulatorMs;
    private double _slowAccumulatorMs;

    #endregion Tick Accumulators

    #region Tick Counters

    public long FastTickCount { get; private set; }
    public long NormalTickCount { get; private set; }
    public long SlowTickCount { get; private set; }

    private const int MaxCatchupTicks = 3;
    
    #endregion Tick Counters

    #region Lifecycle

    private bool _isDisposing;
    
    /// <summary>
    /// Starts the Metronome on a dedicated background thread.
    /// Syncs static interval shortcuts from config so tile math stays accurate.
    /// </summary>
    public void Start()
    {
        if (_running)
            return;

        FastIntervalMs = GameDirector.Config.FastIntervalMs;
        NormalIntervalMs = GameDirector.Config.NormalIntervalMs;
        SlowIntervalMs = GameDirector.Config.SlowIntervalMs;

        _running = true;
        _cts = new CancellationTokenSource();

        _thread = new Thread(() => Loop(_cts.Token))
        {
            Name = "Metronome",
            IsBackground = true,
            Priority = ThreadPriority.AboveNormal
        };

        Log.Debug($"[GameDirector]: Starting Metronome on thread {_thread.ManagedThreadId}");
        _thread.Start();
    }

    /// <summary>
    /// Stops the Metronome and waits for the thread to exit cleanly.
    /// </summary>
    public void Stop()
    {
        if (!_running)
            return;

        _running = false;
        _cts?.Cancel();
        _thread?.Join(timeout: TimeSpan.FromSeconds(2));
        _cts?.Dispose();
        _cts = null;
    }

    public void Dispose()
    {
        if (_isDisposing)
            return;
        
        _isDisposing = true;

        if (_running)
            Stop();
        
        _cts = null;
        _thread = null;
    }
    #endregion Lifecycle

    #region Loop

    private void Loop(CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var lastElapsed = stopwatch.Elapsed;

        while (_running && !ct.IsCancellationRequested)
        {
            var now = stopwatch.Elapsed;
            var deltaMs = (now - lastElapsed).TotalMilliseconds;
            lastElapsed = now;

            _fastAccumulatorMs += deltaMs;
            _normalAccumulatorMs += deltaMs;
            _slowAccumulatorMs += deltaMs;
            
            // Cap it to prevent a death cycle
            _fastAccumulatorMs = Math.Min(_fastAccumulatorMs, FastIntervalMs * MaxCatchupTicks);
            _normalAccumulatorMs = Math.Min(_normalAccumulatorMs, NormalIntervalMs * MaxCatchupTicks);
            _slowAccumulatorMs = Math.Min(_slowAccumulatorMs, SlowIntervalMs * MaxCatchupTicks);
            
            while (_fastAccumulatorMs >= FastIntervalMs)
            {
                _fastAccumulatorMs -= FastIntervalMs;
                FireTick(TickType.Fast);
            }

            while (_normalAccumulatorMs >= NormalIntervalMs)
            {
                _normalAccumulatorMs -= NormalIntervalMs;
                FireTick(TickType.Normal);
            }
            
            while (_slowAccumulatorMs >= SlowIntervalMs)
            {
                _slowAccumulatorMs -= SlowIntervalMs;
                FireTick(TickType.Slow);
            }

            var sleepMs = FastIntervalMs / 2;
            var nextWakeTime = stopwatch.Elapsed + TimeSpan.FromMilliseconds(FastIntervalMs / 2.0);
            if (sleepMs > 2)
                Thread.Sleep(sleepMs - 2);

            while (stopwatch.Elapsed < nextWakeTime)
                Thread.SpinWait(10);
        }
    }

    private void FireTick(TickType tickType)
    {
        switch (tickType)
        {
            case TickType.Fast      : FastTickCount++;        break;
            case TickType.Normal    : NormalTickCount++;      break;
            case TickType.Slow      : SlowTickCount++;        break;
            case TickType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tickType), tickType, null);
        }
        
        GameDirector.ActiveTick(tickType);
        GameDirector.WarmTick(tickType);
    }

    #endregion Loop

    #region Diagnostics

    public override string ToString()
        => $"Metronome Running={_running} Fast={FastTickCount} Normal={NormalTickCount} Slow={SlowTickCount}";
    #endregion Diagnostics
}
/*
 *------------------------------------------------------------
 * (Metronome.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */