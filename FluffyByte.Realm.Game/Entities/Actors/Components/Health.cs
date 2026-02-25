/*
 * (Health.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 22, 2026@11:15:07 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors.Events;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public class Health : GameObjectComponent
{
    public override TickType TickType => TickType.None;
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
        _current = Math.Clamp(_current, 0, _max);

        if (_current <= 0)
            OnDeath();
    }

    public override void Tick()
    {
        
    }

    private void OnDeath()
    {
        if (Owner is null)
        {
            Log.Warn($"[Health Component]: Owner is null during OnDeath.");
            return;
        }

        if (!Owner.HasComponent<ActorComponent>())
            return;
        
        var healthRegen = Owner.GetComponent<HealthRegeneration>();

        if (healthRegen != null)
            healthRegen.Freeze = true;
        
        EventManager.Publish(new ActorDiedEvent() { Actor = Owner });
    }
}

/*
 *------------------------------------------------------------
 * (Health.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */