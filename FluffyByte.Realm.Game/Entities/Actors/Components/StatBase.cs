/*
 * (StatBase.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@4:26:35 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

/// <summary>
/// Represents the base structure for character stats, encapsulating the core attributes that define a character's 
/// capabilities in various aspects of gameplay. Each stat consists of a base value, equipment bonuses, and temporary 
/// modifiers, which together determine the effective stat value and its corresponding modifier used in game mechanics 
/// such as combat, skill checks, and other interactions. This class serves as a foundational component for managing character 
/// attributes and can be extended or customized to accommodate specific game systems or mechanics.
/// </summary>
public class StatBase
{

    /// <summary>
    /// Represents the inherent value of the stat, which is determined by the character's attributes
    /// </summary>
    public int Base { get; set; } = 10;

    /// <summary>
    /// Represents the bonuses or penalties provided by equipped items, such as armor, weapons, accessories, or other gear.
    /// </summary>
    public int Equipment { get; set; } = 0;

    /// <summary>
    /// Represents temporary buffs or debuffs that can affect the stat. These are typically applied through spells, 
    /// abilities, or consumable items and can vary in duration and magnitude. Temporary modifiers are added to the base 
    /// stat and equipment bonuses to calculate the effective stat value during gameplay.
    /// </summary>
    public int Temporary { get; set; } = 0;


    /// <summary>
    /// The effective value of the stat is calculated by summing the base value, equipment bonuses, and temporary buffs.
    /// </summary>
    public int Effective => Math.Clamp(Base + Equipment + Temporary, 1, 30);


    /// <summary>
    /// The modifier is calculated as (Effective - 10) / 2, which is a common formula used in many RPG systems to determine 
    /// the bonus or penalty associated with a particular stat value. A stat of 10 yields a modifier of 0, while higher values 
    /// provide positive modifiers and lower values yield negative modifiers.
    /// </summary>
    public int Modifier => (Effective - 10) / 2;
}

/*
 *------------------------------------------------------------
 * (StatBase.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */