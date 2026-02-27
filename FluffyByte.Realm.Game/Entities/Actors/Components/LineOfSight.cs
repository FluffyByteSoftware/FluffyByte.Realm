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
    private int _sightRange = 50;
    
    public int SightRange
    {
        get
        {
            _sightRange = Math.Clamp(_sightRange, 0, 100);
            
            return _sightRange;
        }
        
        set
        {
            var temp = Math.Clamp(value, 0, 100);
            _sightRange = temp;
        }
    }

    private int _audibleRange;

    public int AudibleRange
    {
        get
        {
            if(_audibleRange == 0 || _audibleRange > _sightRange / 2)
                _audibleRange = Math.Clamp(0, _audibleRange, _sightRange / 2);
            
            return _audibleRange;
        }
        
        set
        {
            var temp = Math.Clamp(value, 0, _sightRange / 2);
            
            _audibleRange = temp;
        }
    }
    
    public override TickType TickType => TickType.None;
}

/*
 *------------------------------------------------------------
 * (LineOfSight.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */