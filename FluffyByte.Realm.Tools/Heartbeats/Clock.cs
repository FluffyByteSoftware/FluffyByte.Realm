/*
 * (Clock.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@1:32:34 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Diagnostics;
using System.Timers;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Tools.Heartbeats;

public class Clock : IDisposable
{
    private readonly string _name;
    private readonly int _tickIntervalMs;
    private readonly System.Timers.Timer _tickTimer;
    private readonly Stopwatch _stopwatch;
    private long _tickCount;
    private DateTime _startTime;
    private bool _disposed;
    private bool _isRunning;

    public event Action? OnTick;
    
    internal Clock(string name, int tickIntervalMs)
    {
        _name = name;
        _tickIntervalMs = tickIntervalMs;
        _stopwatch = new Stopwatch();
        _tickTimer = new System.Timers.Timer(tickIntervalMs);
        _tickTimer.Elapsed += OnTimerElapsed;
        _tickTimer.AutoReset = true;
        
        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
    }

    public string Name => _name;
    public int TickIntervalMs => _tickIntervalMs;
    public long CurrentTick => _tickCount;
    public bool IsRunning => _isRunning;
    public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

    #region Life Cycle
    public void Start()
    {
        if (_isRunning)
        {
            Log.Warn($"Clock: '{_name}' is already running.");
            return;
        }

        _startTime = DateTime.UtcNow;
        _tickCount = 0;
        _stopwatch.Start();
        _tickTimer.Start();
        _isRunning = true;

        Log.Info($"Clock '{_name}' started.");
        
        EventManager.Publish(new ClockStartedEvent());
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            Log.Warn($"Clock '{_name}' is not running.");
            return;
        }

        _tickTimer.Stop();
        _stopwatch.Stop();
        _isRunning = false;

        Log.Info($"Clock '{_name}' stopped after {_tickCount} ticks");
    }

    #endregion Life Cycle
    
    public void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _tickCount++;

        OnTick?.Invoke();
    }

    private void OnSystemShutdown(SystemShutdownEvent e)
    {
        Stop();
    }

    public string FormatUptime()
    {
        var uptime = DateTime.UtcNow - _startTime;
        
        return $"{uptime.Hours:D2}:{uptime.Minutes:D2}:{uptime.Seconds:D2}";
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        OnTick = null;
        
        if(_isRunning)
            Stop();

        _tickTimer.Dispose();
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
    }
}

/*
 *------------------------------------------------------------
 * (Clock.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */