/*
 * (ClockManager.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@1:19:53 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Tools.Heartbeats;

public static class ClockManager
{
    private static readonly ConcurrentDictionary<string, Clock> Clocks = [];

    private static readonly Lock Lock = new Lock();

    public static Clock RegisterClock(string name, int tickIntervalMs)
    {
        lock (Lock)
        {
            if (Clocks.ContainsKey(name))
            {
                Log.Warn($"Clock '{name}' already registered.");
                return Clocks[name];
            }

            var clock = new Clock(name, tickIntervalMs);

            Clocks[name] = clock;
            
            Log.Info($"Registered clock '{name}' with {tickIntervalMs}ms tick interval.");
            return clock;
        }
    }

    public static void UnregisterClock(string name)
    {
        lock (Lock)
        {
            Clocks.TryRemove(name, out _);
        }
    }

    public static void StartClock(string name)
    {
        lock (Lock)
        {
            if (Clocks.TryGetValue(name, out var clock))
            {
                clock.Start();
            }
        }
    }

    public static void StopClock(string name)
    {
        lock (Lock)
        {
            if (Clocks.TryGetValue(name, out var clock) && clock.IsRunning)
            {
                clock.Stop();
            }
        }
    }
}


/*
 *------------------------------------------------------------
 * (ClockManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */