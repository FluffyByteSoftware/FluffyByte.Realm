/*
 * (Regeneration.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@1:19:31 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Brains.Assistants;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public class HealthRegeneration : GameObjectComponent
{
    public override TickType TickType => TickType.Slow;

    /// <summary>
    /// How many real seconds between each "tick".
    /// </summary>
    public int RegenIntervalSeconds { get; set; } = 3;

    /// <summary>
    /// Base amount to regenerate per tick interval.
    /// Calculation is RegenMultiplier * (RegenAmount + Constitution Modifier)
    /// </summary>
    public int RegenAmount { get; set; } = 1;

    /// <summary>
    /// Multiplier to apply to the regen amount.
    /// Calculation is RegenMultiplier * (RegenAmount + Constitution Modifier)
    /// </summary>
    public int RegenMultiplier { get; set; } = 1;

    public bool Freeze { get; set; }
    
    private int _ticksAccumulator;
    private int TicksPerSecond => 1000 / Metronome.SlowIntervalMs;

    public override void Tick()
    {
        if (Owner == null)
            return;

        if (Freeze) return;
        
        _ticksAccumulator++;

        if (_ticksAccumulator < TicksPerSecond * RegenIntervalSeconds)
            return;

        _ticksAccumulator = 0;

        var conMod = Owner.GetComponent<ActorStats>()?.Constitution.Modifier ?? 0;
        
        var regenAmount = RegenMultiplier * (RegenAmount + conMod);
        
        ApplyRegen(regenAmount);
    }

    public void CatchUp(long missedSlowTicks)
    {
        if (Owner == null) return;
        if (Freeze) return;
        
        var missedIntervals = missedSlowTicks / (TicksPerSecond * RegenIntervalSeconds);
        
        if (missedIntervals > 0)
        {
            var conMod = Owner?.GetComponent<ActorStats>()?.Constitution.Modifier ?? 0;
            var regenAmount = RegenMultiplier * (RegenAmount + conMod);
            
            ApplyRegen(regenAmount * (int)missedIntervals);
        }
    }

    private void ApplyRegen(int amount)
    {
        var health = Owner?.GetComponent<Health>();

        health?.Current = Math.Clamp(health.Current + amount, 0, health.Max);
    }
}

/*
 *------------------------------------------------------------
 * (Regeneration.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */