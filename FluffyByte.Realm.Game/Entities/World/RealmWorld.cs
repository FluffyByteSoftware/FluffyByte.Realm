/*
 * (RealmWorld.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@10:33:10 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.World.Zones;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Entities.World;

/// <summary>
/// Represents a world composed of distinct zones, each containing a grid of tiles.
/// The world is structured as a grid of zones, with lazy loading and neighbor linking functionality.
/// </summary>
public class RealmWorld
{
    #region Constants

    /// <summary>
    /// The number of zones in the world (X).
    /// </summary>
    public const int ZoneCountX = 50;
    /// <summary>
    /// The number of zones in the world (Z).
    /// </summary>
    public const int ZoneCountZ = 50;

    #endregion Constants

    #region Grid

    public RealmZone[,] Zones { get; } = new RealmZone[ZoneCountX, ZoneCountZ];

    #endregion Grid

    #region Constructor

    public RealmWorld()
    {
        for (var zx = 0; zx < ZoneCountX; zx++)
        for (var zz = 0; zz < ZoneCountZ; zz++)
            Zones[zx, zz] = new RealmZone($"Zone_{zx}_{zz}",
                worldOffsetX: zx * RealmZone.Width,
                worldOffsetZ: zz * RealmZone.Height);
    }

    #endregion Constructor

    #region Lifecycle

    public void OnLoad()
    {
        // Zones are no longer eagerly loaded — just wire neighbors.
        // Tile allocation happens lazily when WorldComposer needs them.
        WireZoneNeighbors();
    }

    public void OnUnload()
    {
        for (var zx = 0; zx < ZoneCountX; zx++)
        for (var zz = 0; zz < ZoneCountZ; zz++)
            Zones[zx, zz].OnUnload();
    }

    #endregion Lifecycle

    #region Zone Neighbor Wiring

    private void WireZoneNeighbors()
    {
        for (var zx = 0; zx < ZoneCountX; zx++)
        for (var zz = 0; zz < ZoneCountZ; zz++)
        {
            var neighbors = new List<RealmZone>();

            for (var dx = -1; dx <= 1; dx++)
            for (var dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;

                var nx = zx + dx;
                var nz = zz + dz;

                if (nx is >= 0 and < ZoneCountX && nz is >= 0 and < ZoneCountZ)
                    neighbors.Add(Zones[nx, nz]);
            }

            Zones[zx, zz].SetNeighbors([.. neighbors]);
        }
    }

    #endregion Zone Neighbor Wiring

    #region Zone Access

    public RealmZone? TryGetZone(int globalX, int globalZ)
    {
        if (globalX < 0 || globalZ < 0)
            return null;

        var zoneX = globalX / RealmZone.Width;
        var zoneZ = globalZ / RealmZone.Height;

        if (zoneX >= ZoneCountX || zoneZ >= ZoneCountZ)
            return null;

        return Zones[zoneX, zoneZ];
    }

    #endregion Zone Access

    #region Tile Access

    public RealmTile? TryGetTile(int globalX, int globalZ)
    {
        var zone = TryGetZone(globalX, globalZ);
        if (zone is null || !zone.IsLoaded)
            return null;

        var localX = globalX % RealmZone.Width;
        var localZ = globalZ % RealmZone.Height;

        return zone.Tiles[localX, localZ];
    }

    public RealmTile GetTile(int globalX, int globalZ)
        => TryGetTile(globalX, globalZ) ??
           throw new ArgumentOutOfRangeException(
               $"[RealmWorld]: Global tile ({globalX},{globalZ}) is out of bounds or zone not loaded.");

    #endregion Tile Access

    #region Diagnostics

    public override string ToString()
        => $"RealmWorld [{ZoneCountX}x{ZoneCountZ} zones, {ZoneCountX * RealmZone.Width}" +
           $"x{ZoneCountZ * RealmZone.Height} tiles]";

    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (RealmWorld.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */