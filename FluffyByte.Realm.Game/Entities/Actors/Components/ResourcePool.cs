/*
 * (ResourcePool.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@5:52:56 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public abstract class ResourcePool : GameObjectComponent
{
    private int _max = 100;
    private int _current = 100;
    
    public int Max
    {
        get
        {
            CheckCurrent();
            
            return _max;
        }
        set
        {
            _max = value;
            CheckCurrent();
        }
    }
    
    public int Current
    {
        get
        {
            CheckCurrent();
            
            return _current;
        }
        set
        {
            _current = value;
            CheckCurrent();
        }
    }

    private void CheckCurrent()
    {
        if (_current > _max)
            _current = _max;
    }
    
}

/*
 *------------------------------------------------------------
 * (ResourcePool.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */