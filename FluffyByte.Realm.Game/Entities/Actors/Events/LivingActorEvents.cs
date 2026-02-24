/*
 * (LivingActorEvents.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@12:31:24 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors.Events;

public class ActorSpawnEvent : EventArgs
{
    public DateTime SpawnTime { get; init; } = DateTime.UtcNow;
    
}

public class ActorDiedEvent : EventArgs
{
    public GameObject Actor { get; set; }
}

/*
 *------------------------------------------------------------
 * (LivingActorEvents.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */