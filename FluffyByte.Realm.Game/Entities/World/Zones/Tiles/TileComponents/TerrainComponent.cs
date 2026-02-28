/*
 * (TerrainComponent.cs)
 *------------------------------------------------------------
 * Created - 2/28/2026 10:36:24 AM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;

namespace FluffyByte.Realm.Game.Entities.World.Zones.Tiles.TileComponents;

public class TerrainComponent(TerrainType terrainType) : TileComponent
{
    public TerrainType TerrainType { get; set; } = terrainType;

    public override TickType TickType => TickType.None;
}



/*
 *------------------------------------------------------------
 * (TerrainComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */