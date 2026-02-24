/*
 * (ActorStats.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@4:27:34 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public class ActorStats : GameObjectComponent
{
    public override TickType TickType => TickType.None;

    public StatBase Strength { get; set; } = new();
    public StatBase Dexterity { get; set; } = new();
    public StatBase Constitution { get; set; } = new();
    public StatBase Intelligence { get; set; } = new();
    public StatBase Wisdom { get; set; } = new();
    public StatBase Charisma { get; set; } = new();
    
    public override void Tick()
    {
        
    }
    
}

/*
 *------------------------------------------------------------
 * (ActorStats.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */