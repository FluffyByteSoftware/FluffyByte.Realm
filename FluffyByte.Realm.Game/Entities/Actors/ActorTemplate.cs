/*
 * (ActorTemplate.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@9:42:10 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors;

public class ActorTemplate : GameObjectTemplate
{
    public required int CurrentHealth { get; set; }
    public required int MaxHealth { get; set; }
    public required int CurrentTileX { get; set; }
    public required int CurrentTileZ { get; set; }
    
    public int PreviousTileX { get; set; }
    
    public int PreviousTileZ { get; set; }

    public required int LineOfSight { get; set; }
    public required int AudibleRange { get; set; }
    
    public required int Strength { get; set; }
    public required int Dexterity { get; set; }
    public required int Constitution { get; set; }
    public required int Intelligence { get; set; }
    public required int Wisdom { get; set; }
    public required int Charisma { get; set; }
    public required int HealthRegenPerTick { get; set; }
    public required int HealthRegenIntervalSeconds { get; set; }
    public required int HealthRegenMultiplier { get; set; }
}

/*
 *------------------------------------------------------------
 * (ActorTemplate.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */