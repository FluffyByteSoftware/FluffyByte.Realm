/*
 * (Actor.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 22, 2026@7:15:40 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.Interfaces;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Entities.Actors;

public class Actor : GameObject, IGameObjectActor
{
    #region IGameObjectActor

    public int FootPrintRadiusSize
    {
        get => GetComponent<CollisionShapeComponent>()?.FootprintRadius ?? 0;

        set
        {
            var collision = GetComponent<CollisionShapeComponent>();
            
            if (collision != null)
                collision.FootprintRadius = value;
        }
    }
    #endregion IGameObjectActor
    
    #region Component Shortcuts

    public TransformComponent Transform => GetComponent<TransformComponent>()!;
    public ViewModelComponent ViewModel => GetComponent<ViewModelComponent>()!;
    public CollisionShapeComponent Collision => GetComponent<CollisionShapeComponent>()!;

    #endregion Component Shortcuts
    
    #region Constructor

    public Actor(
        string name,
        IGameObjectOwner? owner = null,
        RealmTile? startingTile = null,
        PrimitiveModelType modelType = PrimitiveModelType.Capsule,
        int footprintRadius = 0) : base(name, owner)
    {
        
    }
    #endregion Constructor
    
    #region Diagnostics

    public override string ToString()
        => $"Actor '{Name}' Tile=({Transform.GlobalX}, {Transform.GlobalZ}) " +
           $"Model={ViewModel.ModelType} Footprint ={Collision.FootprintRadius}";
    

    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (Actor.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */