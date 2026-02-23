/*
 * (RealmZone.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@6:22:03 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Events;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
using FluffyByte.Realm.Tools.Broadcasting;

namespace FluffyByte.Realm.Game.Entities.World.Zones;

public class RealmZone(string name, int worldOffsetX, int worldOffsetZ)
{
    #region Constants

    public const int Width  = 128;
    public const int Height = 128;

    #endregion Constants

    #region Identity

    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public int WorldOffsetX { get; } = worldOffsetX;
    public int WorldOffsetZ { get; } = worldOffsetZ;

    #endregion Identity

    #region Tiles

    public RealmTile[,] Tiles { get; } = new RealmTile[Width, Height];

    #endregion Tiles

    #region Neighbors

    public RealmZone[] Neighbors { get; private set; } = [];

    public void SetNeighbors(RealmZone[] neighbors) => Neighbors = neighbors;

    #endregion Neighbors
    
    #region Lifecycle

    public void OnLoad()
    {
        for (var x = 0; x < Width; x++)
        for (var z = 0; z < Height; z++)
            Tiles[x, z] = new RealmTile(x, z, WorldOffsetX + x, WorldOffsetZ + z)
            {
                Zone = this
            };

        WireInternalNeighbors();

        EventManager.Publish(new RealmZoneLoadedEvent { Zone = this });
    }

    public void OnUnload()
    {
        for (var x = 0; x < Width; x++)
        for (var z = 0; z < Height; z++)
            Tiles[x, z].OnColdUnload();

        EventManager.Publish(new RealmZoneUnloadedEvent { Zone = this });
    }

    #endregion Lifecycle

    #region Neighbor Wiring

    private void WireInternalNeighbors()
    {
        for (var x = 0; x < Width; x++)
        for (var z = 0; z < Height; z++)
        {
            var tile      = Tiles[x, z];
            var neighbors = new RealmTile[8];
            var count     = 0;

            for (var dx = -1; dx <= 1; dx++)
            for (var dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;

                var nx = x + dx;
                var nz = z + dz;

                if (nx is >= 0 and < Width && nz is >= 0 and < Height)
                    neighbors[count++] = Tiles[nx, nz];
            }

            tile.SetNeighbors(neighbors[..count]);
        }
    }

    /// <summary>
    /// Called by RealmWorld after all zones and their neighbors are set.
    /// Wires border tiles to their counterparts in adjacent zones.
    /// </summary>
    public void WireBorderNeighbors()
    {
        foreach (var neighbor in Neighbors)
        {
            for (var x = 0; x < Width; x++)
            for (var z = 0; z < Height; z++)
            {
                var tile    = Tiles[x, z];
                var globalX = WorldOffsetX + x;
                var globalZ = WorldOffsetZ + z;

                var extra = new RealmTile[8];
                var count = tile.Neighbors.Length;

                Array.Copy(tile.Neighbors, extra, count);

                for (var dx = -1; dx <= 1; dx++)
                for (var dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0) continue;

                    var ngx = globalX + dx;
                    var ngz = globalZ + dz;

                    var nx = ngx - neighbor.WorldOffsetX;
                    var nz = ngz - neighbor.WorldOffsetZ;

                    if (nx is >= 0 and < Width && nz is >= 0 and < Height)
                        extra[count++] = neighbor.Tiles[nx, nz];
                }

                if (count != tile.Neighbors.Length)
                    tile.SetNeighbors(extra[..count]);
            }
        }
    }

    #endregion Neighbor Wiring

    #region Tile Access

    public RealmTile GetTile(int x, int z) => Tiles[x, z];

    public RealmTile? TryGetTile(int x, int z)
        => x is >= 0 and < Width && z is >= 0 and < Height ? Tiles[x, z] : null;

    #endregion Tile Access

    #region Diagnostics

    public override string ToString()
        => $"RealmZone '{Name}' [{Id}] Offset=({WorldOffsetX},{WorldOffsetZ})";

    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (RealmZone.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */