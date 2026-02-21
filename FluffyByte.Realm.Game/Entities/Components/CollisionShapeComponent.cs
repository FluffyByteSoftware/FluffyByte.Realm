/*
 * (CollisionShapeComponent.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@7:55:00 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;

namespace FluffyByte.Realm.Game.Entities.Components;

public enum CollisionShapeType
{
    Capsule = 0,
    Sphere = 1,
    Box = 2,
    Pyramid = 3,
    Cone = 4
}

public class CollisionShapeComponent : GameObjectComponent
{
    public CollisionShapeType ShapeType { get; set; } = CollisionShapeType.Capsule;
    
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