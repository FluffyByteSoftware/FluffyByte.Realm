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

public class RealmWorld
{
    #region Constants

    public const int ZoneCountX = 2;
    public const int ZoneCountZ = 2;

    #endregion Constants
    
    #region Grid

    public RealmZone[,] Zones { get; } = new RealmZone[ZoneCountX, ZoneCountZ];
    #endregion Grid
    
    #region Constructor

    public RealmWorld()
    {
        Zones[0, 0] = new RealmZone("Zone1", worldOffsetX: 0,               worldOffsetZ: 0);
        Zones[1, 0] = new RealmZone("Zone2", worldOffsetX: RealmZone.Width, worldOffsetZ: 0);
        Zones[0, 1] = new RealmZone("Zone3", worldOffsetX: 0,               worldOffsetZ: RealmZone.Height);
        Zones[1, 1] = new RealmZone("Zone4", worldOffsetX: RealmZone.Width, worldOffsetZ: RealmZone.Height);
    }
    #endregion Constructor
    
    #region Lifecycle

    public void OnLoad()
    {
        for(var zx = 0; zx < ZoneCountX; zx++)
        for (var zz = 0; zz < ZoneCountZ; zz++)
            Zones[zx, zz].OnLoad();

        WireZoneNeighbors();
        
        for(var zx = 0; zx < ZoneCountX; zx++)
        for (var zz = 0; zz < ZoneCountZ; zz++)
            Zones[zx, zz].WireBorderNeighbors();
    }

    public void OnUnload()
    {
        for(var zx = 0; zx < ZoneCountX; zx++)
        for (var zz = 0; zz < ZoneCountZ; zz++)
            Zones[zx, zz].OnUnload();
    }
    #endregion Lifecycle
    
    #region Zone Neighbor Wiring

    private void WireZoneNeighbors()
    {
        for(var zx = 0; zx < ZoneCountX; zx++)
        for (var zz = 0; zz < ZoneCountZ; zz++)
        {
            var neighbors = new List<RealmZone>();
            
            for(var dx = -1;dx <= 1; dx++)
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
    
    #region Tile Access

    public RealmTile? TryGetTile(int globalX, int globalZ)
    {
        var zoneX = globalX / RealmZone.Width;
        var zoneZ = globalZ / RealmZone.Height;

        if (zoneX < 0 || zoneX >= ZoneCountX || zoneZ < 0 || zoneZ >= ZoneCountZ)
            return null;

        var localX = globalX % RealmZone.Width;
        var localZ = globalZ % RealmZone.Height;
        
        return Zones[zoneX, zoneZ].Tiles[localX, localZ];
    }

    public RealmTile GetTile(int globalX, int globalZ)
        => TryGetTile(globalX, globalZ) ??
           throw new ArgumentOutOfRangeException(
               $"[RealmWorld]: Global title ({globalX},{globalZ}) is out of bounds");

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