/*
 * (LineOfSight.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@11:52:23 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public class LineOfSight : GameObjectComponent
{
    public int SightRange { get; set; } = 100;
    public int AudibleRange { get; set; } = 50;
    
    public override TickType TickType => TickType.None;
}

/*
 *------------------------------------------------------------
 * (LineOfSight.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */