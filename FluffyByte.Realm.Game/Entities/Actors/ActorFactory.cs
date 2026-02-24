/*
 * (ActorFactory.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@12:35:17 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Text;
using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.Interfaces;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Entities.Actors;

public static class ActorFactory
{
    public static IUniqueActor CreateUniqueActor(string name,
        int health = 100, int mana = 100,
        int strength = 10, int dexterity = 10, int constitution = 10,
        int intelligence = 10, int wisdom = 10, int charisma = 10,
        RealmTile? startingTile = null,
        PrimitiveModelType modelType = PrimitiveModelType.Capsule,
        int footPrintRadius = 1)
    {
        var actor = new UniqueActor(name);
        
        actor.AddComponent(new ActorComponent());
        actor.AddComponent(new TransformComponent(startingTile));
        actor.AddComponent(new ViewModelComponent(modelType));
        actor.AddComponent(new CollisionShapeComponent(footPrintRadius));
        actor.AddComponent(new Health() { Current = health, Max = health});
        actor.AddComponent(new Mana() { Current = mana, Max = mana});

        var stats = new ActorStats
        {
            Strength =
            {
                Base = (byte)strength
            },
            Dexterity =
            {
                Base = (byte)dexterity
            },
            Constitution =
            {
                Base = (byte)constitution
            },
            Intelligence =
            {
                Base = (byte)intelligence
            },
            Wisdom =
            {
                Base = (byte)wisdom
            },
            Charisma =
            {
                Base = (byte)charisma
            }
        };

        // add any other components that distinguish a living actor from other forms

        return actor;
    }
    
    public static GameObject CreateLivingActor(string name,
        int health = 100, int mana = 100,
        int strength = 10, int dexterity = 10, int constitution = 10,
        int intelligence = 10, int wisdom = 10, int charisma = 10,
        RealmTile? startingTile = null,
        PrimitiveModelType modelType = PrimitiveModelType.Capsule,
        int footPrintRadius = 1)
    {
        var actor = new GameObject(name);

        actor.AddComponent(new ActorComponent());
        actor.AddComponent(new TransformComponent(startingTile));
        actor.AddComponent(new ViewModelComponent(modelType));
        actor.AddComponent(new CollisionShapeComponent(footPrintRadius));
        actor.AddComponent(new Health() { Current = health, Max = health});
        actor.AddComponent(new Mana() { Current = mana, Max = mana});

        var stats = new ActorStats
        {
            Strength =
            {
                Base = (byte)strength
            },
            Dexterity =
            {
                Base = (byte)dexterity
            },
            Constitution =
            {
                Base = (byte)constitution
            },
            Intelligence =
            {
                Base = (byte)intelligence
            },
            Wisdom =
            {
                Base = (byte)wisdom
            },
            Charisma =
            {
                Base = (byte)charisma
            }
        };

        // add any other components that distinguish a living actor from other forms

        return actor;
    }
}

/*
 *------------------------------------------------------------
 * (ActorFactory.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */