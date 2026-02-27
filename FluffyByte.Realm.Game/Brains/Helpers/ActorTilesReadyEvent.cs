/*
 * (ActorTilesReadyEvent.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 11:12:35 AM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Brains.Helpers
{
    public class ActorTilesReadyEvent : EventArgs
    {
        public required GameObject Actor { get; init; }
        public required IReadOnlySet<RealmTile> HotTiles { get; init; }
        public required IReadOnlySet<RealmTile> WarmTiles { get; init; }
    }
}

/*
 *------------------------------------------------------------
 * (ActorTilesReadyEvent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */