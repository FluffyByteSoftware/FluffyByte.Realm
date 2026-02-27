/*
 * (ActorFactory.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@12:35:17 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;
using FluffyByte.Realm.Shared.Misc;
using System.Numerics;

namespace FluffyByte.Realm.Game.Entities.Actors;

public static class ActorFactory
{
    #region Player

    public static GameObject CreatePlayerActor(PlayerProfile profile)
    {
        GameObject actor = new(profile.Name)
        {
            UniqueObjectType = UniqueObjectType.Player
        };

        actor.AddComponent(new TransformComponent());

        actor.AddComponent(new ViewModelComponent(profile.ModelType)
        {
            RealModelType = profile.ComplexModelType ?? ComplexModelType.DefaultAndrogynous
        });

        actor.AddComponent(new CollisionShapeComponent
        {
            FootprintRadius = profile.FootprintRadius
        });

        actor.AddComponent(new LineOfSight
        {
            SightRange = profile.LineOfSight,
            AudibleRange = profile.AudibleRange
        });

        actor.AddComponent(new ActorStats
        {
            Strength = { Base = profile.Strength },
            Dexterity = { Base = profile.Dexterity },
            Constitution = { Base = profile.Constitution },
            Intelligence = { Base = profile.Intelligence },
            Wisdom = { Base = profile.Wisdom },
            Charisma = { Base = profile.Charisma }
        });

        actor.AddComponent(new Health
        {
            Current = profile.CurrentHealth,
            Max = profile.MaxHealth
        });

        actor.AddComponent(new HealthRegeneration
        {
            RegenAmount = profile.HealthRegenPerTick,
            RegenIntervalSeconds = profile.HealthRegenIntervalSeconds,
            RegenMultiplier = profile.HealthRegenMultiplier
        });

        actor.AddComponent(new ActorComponent());
       
        return actor;
    }

    public static void ExtractToProfile(GameObject actor, PlayerProfile profile)
    {
        var transform = actor.GetComponent<TransformComponent>();
        var health = actor.GetComponent<Health>();
        var stats = actor.GetComponent<ActorStats>();
        var viewModel = actor.GetComponent<ViewModelComponent>();
        var collision = actor.GetComponent<CollisionShapeComponent>();
        var lineOfSight = actor.GetComponent<LineOfSight>();
        var healthRegen = actor.GetComponent<HealthRegeneration>();

        profile.Name = actor.Name;
        profile.ModelType = viewModel?.ModelType ?? profile.ModelType;
        profile.ComplexModelType = viewModel?.RealModelType;
        profile.FootprintRadius = collision?.FootprintRadius ?? profile.FootprintRadius;
        profile.CurrentHealth = health?.Current ?? profile.CurrentHealth;
        profile.MaxHealth = health?.Max ?? profile.MaxHealth;
        profile.CurrentTileX = transform?.Tile?.GlobalX ?? profile.CurrentTileX;
        profile.CurrentTileZ = transform?.Tile?.GlobalZ ?? profile.CurrentTileZ;
        profile.LineOfSight = lineOfSight?.SightRange ?? profile.LineOfSight;
        profile.AudibleRange = lineOfSight?.AudibleRange ?? profile.AudibleRange;
        profile.Strength = stats?.Strength.Base ?? profile.Strength;
        profile.Dexterity = stats?.Dexterity.Base ?? profile.Dexterity;
        profile.Constitution = stats?.Constitution.Base ?? profile.Constitution;
        profile.Intelligence = stats?.Intelligence.Base ?? profile.Intelligence;
        profile.Wisdom = stats?.Wisdom.Base ?? profile.Wisdom;
        profile.Charisma = stats?.Charisma.Base ?? profile.Charisma;
        profile.HealthRegenPerTick = healthRegen?.RegenAmount ?? profile.HealthRegenPerTick;
        profile.HealthRegenIntervalSeconds = healthRegen?.RegenIntervalSeconds ?? profile.HealthRegenIntervalSeconds;
        profile.HealthRegenMultiplier = healthRegen?.RegenMultiplier ?? profile.HealthRegenMultiplier;
    }

    #endregion Player

    #region NPCs / Bosses

    public static GameObject CreateActor(ActorTemplate template)
        => CreateCoreActor(template);

    public static GameObject CreateWorldBoss(ActorTemplate template)
    {
        var actor = CreateCoreActor(template);
        actor.UniqueObjectType = UniqueObjectType.WorldBoss;
        return actor;
    }

    public static GameObject CreateEliteActor(ActorTemplate template)
    {
        var actor = CreateCoreActor(template);
        actor.UniqueObjectType = UniqueObjectType.EliteNpc;
        return actor;
    }

    public static ActorTemplate ExtractTemplate(GameObject actor)
    {
        var transform = actor.GetComponent<TransformComponent>();
        var health = actor.GetComponent<Health>();
        var stats = actor.GetComponent<ActorStats>();
        var viewModel = actor.GetComponent<ViewModelComponent>();
        var collision = actor.GetComponent<CollisionShapeComponent>();
        var lineOfSight = actor.GetComponent<LineOfSight>();
        var healthRegen = actor.GetComponent<HealthRegeneration>();

        return new ActorTemplate
        {
            Id = actor.Id,
            Name = actor.Name,
            ModelType = viewModel?.ModelType ?? PrimitiveModelType.Capsule,
            ComplexModelType = viewModel?.RealModelType ?? ComplexModelType.DefaultAndrogynous,
            FootprintRadius = collision?.FootprintRadius ?? 1,
            CurrentHealth = health?.Current ?? 0,
            MaxHealth = health?.Max ?? 0,
            CurrentTileX = transform?.Tile?.GlobalX ?? 0,
            CurrentTileZ = transform?.Tile?.GlobalZ ?? 0,
            LineOfSight = lineOfSight?.SightRange ?? 10,
            AudibleRange = lineOfSight?.AudibleRange ?? 10,
            Strength = stats?.Strength.Base ?? 10,
            Dexterity = stats?.Dexterity.Base ?? 10,
            Constitution = stats?.Constitution.Base ?? 10,
            Intelligence = stats?.Intelligence.Base ?? 10,
            Wisdom = stats?.Wisdom.Base ?? 10,
            Charisma = stats?.Charisma.Base ?? 10,
            HealthRegenPerTick = healthRegen?.RegenAmount ?? 1,
            HealthRegenIntervalSeconds = healthRegen?.RegenIntervalSeconds ?? 3,
            HealthRegenMultiplier = healthRegen?.RegenMultiplier ?? 1
        };
    }

    #endregion NPCs / Bosses

    #region Core Builder

    private static GameObject CreateCoreActor(ActorTemplate template)
    {
        var actor = new GameObject(template.Name);

        actor.AddComponent(new TransformComponent());

        actor.AddComponent(new ViewModelComponent(template.ModelType)
        {
            RealModelType = template.ComplexModelType ?? ComplexModelType.DefaultAndrogynous
        });

        actor.AddComponent(new CollisionShapeComponent
        {
            FootprintRadius = template.FootprintRadius
        });

        actor.AddComponent(new LineOfSight
        {
            SightRange = template.LineOfSight,
            AudibleRange = template.AudibleRange
        });

        actor.AddComponent(new ActorStats
        {
            Strength = { Base = template.Strength },
            Dexterity = { Base = template.Dexterity },
            Constitution = { Base = template.Constitution },
            Intelligence = { Base = template.Intelligence },
            Wisdom = { Base = template.Wisdom },
            Charisma = { Base = template.Charisma }
        });

        actor.AddComponent(new Health
        {
            Current = template.CurrentHealth,
            Max = template.MaxHealth
        });

        actor.AddComponent(new HealthRegeneration
        {
            RegenAmount = template.HealthRegenPerTick,
            RegenIntervalSeconds = template.HealthRegenIntervalSeconds,
            RegenMultiplier = template.HealthRegenMultiplier
        });

        actor.AddComponent(new ActorComponent());

        return actor;
    }

    #endregion Core Builder
}

/*
 *------------------------------------------------------------
 * (ActorFactory.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */