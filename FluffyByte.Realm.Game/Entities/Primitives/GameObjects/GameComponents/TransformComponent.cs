/*
 * (Transform.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@7:49:33 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Security.Cryptography.X509Certificates;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

public class TransformComponent : GameObjectComponent
{
    #region Tick Type

    public override TickType TickType => TickType.Fast;

    #endregion Tick Type
    
    #region Spatial State

    public RealmTile? Tile { get; set; }
    public float Rotation { get; set; } = 0f;
    public float Scale { get; set; } = 1f;

    #endregion Spatial State
    
    #region Convenience

    public int GlobalX => Tile?.GlobalX ?? 0;
    public int GlobalZ => Tile?.GlobalZ ?? 0;

    #endregion Convenience
    
    #region Constructor

    public TransformComponent(RealmTile? tile = null, float rotation = 0f, float scale = 1f)
    {
        Tile = tile;
        Rotation = rotation;
        Scale = scale;
    }
    
    #endregion Constructor
    
    #region Lifecycle

    public override void OnSpawn()
    {
        
    }

    public override void OnDestroy()
    {
        Tile = null;
    }
    #endregion Lifecycle
    
    #region Tick
    public override void Tick() {}
    #endregion Tick
    
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