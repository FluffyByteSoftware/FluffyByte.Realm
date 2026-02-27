/*
 * (Transform.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@7:49:33 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

public class TransformComponent(RealmTile? tile = null, float rotation = 0f, int scale = 1) : GameObjectComponent
{
    #region Tick Type

    public override TickType TickType => TickType.Fast;

    #endregion Tick Type

    #region Spatial State

    /// <summary>
    /// The tile the object is currently on
    /// </summary>
    public RealmTile? Tile { get; set; } = tile;

    /// <summary>
    /// Rotation in radians of the Y axis (Yaw)
    /// </summary>
    public float Rotation { get; set; } = rotation;

    /// <summary>
    /// Scale of the object, always in whole numbers.
    /// </summary>
    public int Scale { get; set; } = scale;

    #endregion Spatial State

    #region Convenience

    public int GlobalX => Tile?.GlobalX ?? 0;
    public int GlobalZ => Tile?.GlobalZ ?? 0;

    #endregion Convenience
    #region Constructor

    #endregion Constructor

    #region Lifecycle
    public override void OnDestroy()
    {
        Tile = null;
    }
    #endregion Lifecycle
    
    #region Tick
    public override void Tick() {}
    #endregion Tick
    
    #region Movement

    public Task<RealmTile?>? RequestMove(int globalX, int globalZ)
    {
        if(Owner == null)
        {
            return null;
        }

        return GameDirector.RequestMove(Owner, globalX, globalZ);
    }
    
    #endregion Movement
    #region Diagnostics

    public override string ToString()
        => $"TransformComponent Tile=({GlobalX}, {GlobalZ}) Rotation={Rotation} Scale={Scale}";
    
    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (Transform.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */