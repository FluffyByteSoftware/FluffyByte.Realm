/*
 * (RealmZoneEvents.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@8:50:15 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.World.Zones;

namespace FluffyByte.Realm.Game.Entities.Events;

public class RealmZoneLoadedEvent : EventArgs
{
    public required RealmZone Zone { get; init; }
}

public class RealmZoneUnloadedEvent : EventArgs
{
    public required RealmZone Zone { get; init; }
}

/*
 *------------------------------------------------------------
 * (RealmZoneEvents.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */