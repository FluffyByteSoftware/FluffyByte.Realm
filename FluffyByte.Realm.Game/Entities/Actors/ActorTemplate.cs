/*
 * (ActorTemplate.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@9:42:10 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors;

public class ActorTemplate : GameObjectTemplate
{
    public required int CurrentHealth { get; init; }
    public required int MaxHealth { get; init; }
    public required int CurrentTileX { get; set; }
    public required int CurrentTileZ { get; set; }
    
    public required int PreviousTileX { get; set; }
    
    public required int PreviousTileZ { get; set; }

    public required int LineOfSight { get; init; }
    public required int AudibleRange { get; init; }
    
    public required int Strength { get; init; }
    public required int Dexterity { get; init; }
    public required int Constitution { get; init; }
    public required int Intelligence { get; init; }
    public required int Wisdom { get; init; }
    public required int Charisma { get; init; }
    public required int HealthRegenPerTick { get; init; }
    public required int HealthRegenIntervalSeconds { get; init; }
    public required int HealthRegenMultiplier { get; init; }
}

/*
 *------------------------------------------------------------
 * (ActorTemplate.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */