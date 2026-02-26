/*
 * (ActorFactory.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@12:35:17 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;
using FluffyByte.Realm.Shared.Misc;

namespace FluffyByte.Realm.Game.Entities.Actors;

public static class ActorFactory
{
    public static GameObject CreateActor(ActorTemplate template)
    {
        var newActor = CreateCoreActor(template);
        
        return newActor;
    }
    
    public static GameObject CreateWorldBoss(ActorTemplate template)
    {
        var newActor = CreateCoreActor(template);
        
        newActor.UniqueObjectType = UniqueObjectType.WorldBoss;
        
        return newActor;
    }

    public static GameObject CreatePlayerActor(ActorTemplate template)
    {
        var newPlayer = CreateCoreActor(template);
        
        newPlayer.UniqueObjectType = UniqueObjectType.Player;
        
        return newPlayer;
    }

    public static GameObject CreateEliteActor(ActorTemplate template)
    {
        var newActor = CreateCoreActor(template);
        
        newActor.UniqueObjectType = UniqueObjectType.EliteNPC;
        
        return newActor;
    }

    private static GameObject CreateCoreActor(ActorTemplate template)
    {
        
        var actor = new GameObject(template.Name);
        
        actor.AddComponent(new TransformComponent());

        var viewComp = new ViewModelComponent(template.ModelType)
        {
            RealModelType = ComplexModelType.DefaultAndrogynous
        };

        actor.AddComponent(viewComp);

        var collision = new CollisionShapeComponent() { FootprintRadius = template.FootprintRadius };
        actor.AddComponent(collision);
        
        var actorStats = new ActorStats
        {
            Strength =
            {
                Base = template.Strength
            },
            Dexterity =
            {
                Base = template.Dexterity
            },
            Constitution =
            {
                Base = template.Constitution
            },
            Intelligence =
            {
                Base = template.Intelligence
            },
            Wisdom =
            {
                Base = template.Wisdom
            },
            Charisma =
            {
                Base = template.Charisma
            }
        };

        actor.AddComponent(actorStats);
        
        var health = new Health()
        {
            Current = 1,
            Max = 100
        };
        
        actor.AddComponent(health);
        
        var healthRegen = new HealthRegeneration()
        {
            RegenAmount = 1,
            RegenIntervalSeconds = 2,
            RegenMultiplier = 1
        };
        
        actor.AddComponent(healthRegen);

        var actComp = new ActorComponent();
        actor.AddComponent(actComp);
        
        return actor;
    }
}

/*
 *------------------------------------------------------------
 * (ActorFactory.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */