/*
 * (CollisionShapeComponent.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@7:55:00 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

public enum CollisionShapeType
{
    Circle = 0,
    Rectangle = 1
}

public class CollisionShapeComponent : GameObjectComponent
{
    public CollisionShapeType ShapeType { get; set; } = CollisionShapeType.Rectangle;
    
    
    public override TickType TickType => TickType.Fast;

    public override void Tick()
    {
        
    }
}

/*
 *------------------------------------------------------------
 * (CollisionShapeComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */