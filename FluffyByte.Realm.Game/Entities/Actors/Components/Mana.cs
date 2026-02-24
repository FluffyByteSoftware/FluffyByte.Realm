/*
 * (GuildPoints.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@9:47:40 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public class Mana : ResourcePool
{
    public override TickType TickType => TickType.None;
    
    public override void Tick()
    {
        
    }
}

/*
 *------------------------------------------------------------
 * (GuildPoints.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */