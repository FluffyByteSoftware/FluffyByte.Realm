/*
 * (ActorTilesChangedEvent.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 12:27:48 PM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Entities.Actors.Events
{
    public class ActorTilesChangedEvent : EventArgs
    {
        public required GameObject Actor { get; set; }
        public required bool IsFirstRefresh { get; set; }
        public required IReadOnlySet<RealmTile> HotTiles { get; set; }
        public required IReadOnlySet<RealmTile> WarmTiles { get; set; }
        public required IReadOnlySet<RealmTile> PreviousHotTiles { get; set; }
        public required IReadOnlySet<RealmTile> PreviousWarmTiles { get; set; }
    }
}

/*
 *------------------------------------------------------------
 * (ActorTilesChangedEvent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */