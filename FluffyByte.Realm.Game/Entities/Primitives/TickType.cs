/*
 * (TickType.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 19, 2026@4:46:30 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Primitives;

/// <summary>
/// Specifies the available tick speeds for timing intervals.
/// </summary>
/// <remarks>Use this enumeration to select the desired tick rate for timing-related operations. The values
/// represent different speeds, ranging from no tick to fast, normal, and slow intervals. This can be useful for
/// configuring timers, animations, or other time-dependent functionality.</remarks>
public enum TickType
{
    /// <summary>
    /// No ticks on this object/component
    /// </summary>
    None = -1,
    /// <summary>
    /// Tick at the fastest rate possible
    /// </summary>
    Fast = 0,
    /// <summary>
    /// Tick at a normal rate
    /// </summary>
    Normal = 1,
    /// <summary>
    /// Tick at a slow rate (750ms - 1s per tick)
    /// </summary>
    Slow = 2,
    /// <summary>
    /// Tick at a very slow rate (1 / minute or more per tick)
    /// </summary>
    VerySlow = 3
}

/*
 *------------------------------------------------------------
 * (TickType.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */