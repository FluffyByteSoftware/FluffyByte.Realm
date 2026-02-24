/*
 * (ActorComponent.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@7:28:55 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public class ActorComponent : GameObjectComponent
{
    public bool IsActor { get; private set; } = true;

    public override TickType TickType => TickType.None;

    public override void Tick()
    {
        
    }
}

/*
 *------------------------------------------------------------
 * (ActorComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */