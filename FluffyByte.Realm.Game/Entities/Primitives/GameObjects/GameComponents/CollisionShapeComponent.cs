/*
 * (CollisionShapeComponent.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@7:55:00 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

public class CollisionShapeComponent : GameObjectComponent
{
    #region Required

    public override TickType TickType => TickType.None;

    #endregion Required

    public int FootprintRadius { get; set; } = 0;

    public CollisionShapeComponent(int footprintRadius = 0)
    {
        FootprintRadius = footprintRadius;
    }

    public override void Tick()
    {
        
    }

    public override string ToString()
        => $"CollisionShapeComponent FootprintRadius={FootprintRadius}";
}

/*
 *------------------------------------------------------------
 * (CollisionShapeComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */