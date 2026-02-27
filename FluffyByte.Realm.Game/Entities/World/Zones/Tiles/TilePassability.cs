/*
 * (TilePassability.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 26, 2026@12:15:02 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
[Flags]
public enum TilePassability : byte
{
    Passable    = 0,
    Flyable     = 1 << 0,
    NotPassable = 1 << 1,
}

/*
 *------------------------------------------------------------
 * (TilePassability.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */