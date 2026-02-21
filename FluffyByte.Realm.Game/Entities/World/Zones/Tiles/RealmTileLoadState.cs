/*
 * (TileType.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@1:01:23 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.World.Zones.Tiles;


/// <summary>
/// Indicates the activity level of the tile.
/// If a tile is Cold, it is inactive and will only execute a SleepyTick.
/// If a tile is Warm, it is "spawning" and will execute a catch up tick.
/// If a tile is Hot, is it active, and will execute ticks as normal.
/// </summary>
public enum RealmTileLoadState
{
    /// <summary>
    /// Represents a tile state where the tile is inactive and performs minimal operations.
    /// A tile in the Cold state executes only a SleepyTick, indicating that it is not actively engaged
    /// in normal game logic or behavior. This state is typically used to conserve resources
    /// for tiles that are not currently relevant or in active use.
    /// </summary>
    Cold,

    /// <summary>
    /// Represents a tile state where the tile is in a transitional "spawning" phase.
    /// In the Warm state, the tile performs a catch-up tick, allowing it to process
    /// delayed logic or events that occurred while it was not in the active state.
    /// This state is often used as an intermediary step between Cold and Hot states,
    /// preparing the tile for normal activity.
    /// </summary>
    Warm,

    /// <summary>
    /// Represents a tile state where the tile is fully active and operating at its maximum capacity.
    /// A tile in the Hot state executes ticks as normal, actively engaging with game logic,
    /// entities, and events. This state is typically assigned to tiles that are in use
    /// or have high gameplay relevance, ensuring their full functionality.
    /// </summary>
    Hot
}
/*
 *------------------------------------------------------------
 * (TileType.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */