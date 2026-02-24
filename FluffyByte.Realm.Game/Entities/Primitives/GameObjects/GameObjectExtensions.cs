/*
 * (GameObjectExtensions.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@12:54:03 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

public static class GameObjectExtensions
{
    public static TransformComponent? GetTransform(this GameObject go)
    {
        return go.GetComponent<TransformComponent>();
    }

    public static ActorComponent? GetActor(this GameObject go)
    {
        return go.GetComponent<ActorComponent>();
    }
    public static CollisionShapeComponent? GetCollision(this GameObject go)
    {
        return go.GetComponent<CollisionShapeComponent>();
    }

    public static bool IsInSameZoneAs(this GameObject me, GameObject other)
    {
        var myTransform = me.GetTransform();
        var otherTransform = other.GetTransform();

        if (myTransform?.Tile == null || otherTransform?.Tile == null)
            return false;

        if (myTransform?.Tile?.Zone == null || otherTransform?.Tile?.Zone == null)
            return false;
        
        return myTransform.Tile.Zone.Id == otherTransform.Tile.Zone.Id;
    }
    
}

/*
 *------------------------------------------------------------
 * (GameObjectExtensions.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */