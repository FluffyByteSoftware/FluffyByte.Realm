/*
 * (ZoneTickFastEvent.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@1:43:14 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.Zones;

namespace FluffyByte.Realm.Game.Entities.Primitives.Events;

/// <summary>
/// Published by a RealmZone every Fast tick.
/// Subscribe to hook custom behavior into the fast tick cycle.
/// </summary>
public class ZoneTickFastEvent : EventArgs
{
    public required RealmZone Zone { get; init; }
}

/// <summary>
/// Published by a RealmZone every Normal tick.
/// Subscribe to hook custom behavior into the normal tick cycle.
/// </summary>
public class ZoneTickNormalEvent : EventArgs
{
    public required RealmZone Zone { get; init; }
}

/// <summary>
/// Published by a RealmZone every Slow tick.
/// Subscribe to hook custom behavior into the slow tick cycle.
/// </summary>
public class ZoneTickSlowEvent : EventArgs
{
    public required RealmZone Zone { get; init; }
}
/*
 *------------------------------------------------------------
 * (ZoneTickFastEvent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */