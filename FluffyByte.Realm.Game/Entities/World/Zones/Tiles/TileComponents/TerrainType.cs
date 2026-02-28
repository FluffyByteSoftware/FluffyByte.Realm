/*
 * (TerrainType.cs)
 *------------------------------------------------------------
 * Created - 2/28/2026 10:34:54 AM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.World.Zones.Tiles.TileComponents;

/// <summary>
/// Specifies the types of terrain that can be represented within a zone or tile.
/// </summary>
/// <remarks>Use this enumeration to categorize terrain for rendering, movement, and gameplay logic. Each value
/// corresponds to a distinct terrain characteristic, which may affect movement speed, visibility, or interaction rules
/// in the game world.</remarks>
public enum TerrainType : byte
{
    /// <summary>
    /// Type grass regular
    /// </summary>
    Grass,
    /// <summary>
    /// Type dirt regular
    /// </summary>
    Dirt,
    /// <summary>
    /// Type sand regular
    /// </summary>
    Sand,
    /// <summary>
    /// Type deep water regular
    /// </summary>
    DeepWater,
    /// <summary>
    /// Type shallow water regular
    /// </summary>
    ShallowWater,

    /// <summary>
    /// Type rock regular
    /// </summary>
    Rock
}



/*
 *------------------------------------------------------------
 * (TerrainType.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */