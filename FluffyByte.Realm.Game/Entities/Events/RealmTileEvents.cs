/*
 * (TileEvents.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@12:53:37 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Entities.Events;

public class RealmTileLoadStateChangedEvent
{
    public required RealmTile Tile { get; init; }
    public required RealmTileLoadState OldState { get; init; }
    public required RealmTileLoadState NewState { get; init; }
}

public class RealmTileEnterTileEvent
{
    public required RealmTile Tile { get; init; }
    public required GameObject ObjectEntering { get; init; }
}

public class RealmTileExitTileEvent
{
    public required RealmTile Tile { get; init; }
    public required GameObject ObjectLeaving { get; init; }
}

/*
 *------------------------------------------------------------
 * (TileEvents.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */