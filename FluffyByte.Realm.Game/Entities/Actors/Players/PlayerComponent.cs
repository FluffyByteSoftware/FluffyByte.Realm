/*
 * (PlayerComponent.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@10:45:10 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Shared.Misc;

namespace FluffyByte.Realm.Game.Entities.Actors.Players;

public class PlayerComponent : GameObjectComponent
{
    public override TickType TickType => TickType.None;

    // Network
    public RealmClient? Client { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public Guid SessionId { get; set; }
    public DateTime LoginTime { get; set; }

    // Identity
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }

    // Appearance
    public required PrimitiveModelType ModelType { get; set; }
    public ComplexModelType? ComplexModelType { get; set; }
    public required int FootprintRadius { get; set; }
    public List<string> Tags { get; set; } = [];

    // Position
    public required int CurrentTileX { get; set; }
    public required int CurrentTileZ { get; set; }
    public int PreviousTileX { get; set; }
    public int PreviousTileZ { get; set; }

    // Vitals
    public required int CurrentHealth { get; set; }
    public required int MaxHealth { get; set; }

    // Stats
    public required int Strength { get; set; }
    public required int Dexterity { get; set; }
    public required int Constitution { get; set; }
    public required int Intelligence { get; set; }
    public required int Wisdom { get; set; }
    public required int Charisma { get; set; }

    // Regen
    public required int HealthRegenPerTick { get; set; }
    public required int HealthRegenIntervalSeconds { get; set; }
    public required int HealthRegenMultiplier { get; set; }

    // Perception
    public required int LineOfSight { get; set; }
    public required int AudibleRange { get; set; }

}

/*
 *------------------------------------------------------------
 * (PlayerComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */