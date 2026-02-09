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
}


/*
 *------------------------------------------------------------
 * (ClockManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */