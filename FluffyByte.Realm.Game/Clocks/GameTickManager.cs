/*
 * (GaneTickManager.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@11:45:16 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Heartbeats;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Clocks;

public static class GameTickManager
{
    private const int FastIntervalMs = 20;
    private const int NormalIntervalMs = 50;
    private const int SlowIntervalMs = 1000;

    private static bool _isInitialized;
    
    #region Life Cycle

    public static void Initialize()
    {
        if (_isInitialized)
            return;

        EventManager.Subscribe<SystemStartupEvent>(OnStart);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
        ClockManager.RegisterClock("GameTick-Fast", FastIntervalMs);
        ClockManager.RegisterClock("GameTick-Normal", NormalIntervalMs);
        ClockManager.RegisterClock("GameTick-Slow", SlowIntervalMs);
        
        _isInitialized = true;
        
        Log.Debug($"[GameTickManager]: Initialized.");
    }

    private static void OnStart(SystemStartupEvent e)
    {
        if(!_isInitialized) return;
        
        ClockManager.StartClock("GameTick-Fast");
        ClockManager.StartClock("GameTick-Normal");
        ClockManager.StartClock("GameTick-Slow");
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized)
            return;
        
        ClockManager.StopClock("GameTick-Fast");
        ClockManager.StopClock("GameTick-Normal");
        ClockManager.StopClock("GameTick-Slow");

        _isInitialized = false;
    }
    #endregion Life Cycle
}

/*
 *------------------------------------------------------------
 * (GaneTickManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */