/*
 * (Transform.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@7:49:33 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

public class TransformComponent : GameObjectComponent
{
    public float X { get; set; }
    public float Z { get; set; }
    public int Y { get; set; }

    public float Rotation { get; set; }
    
    public override TickType TickType => TickType.Fast;

    public override void Tick()
    {
        
    }
}

/*
 *------------------------------------------------------------
 * (Transform.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */