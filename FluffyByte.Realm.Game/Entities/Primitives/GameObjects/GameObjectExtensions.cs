/*
 * (GameObjectExtensions.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@12:54:03 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

public static class GameObjectExtensions
{
    public static TransformComponent? GetTransform(this GameObject go)
    {
        return go.GetComponent<TransformComponent>();
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

        return myTransform.Tile == otherTransform.Tile;
    }
}

/*
 *------------------------------------------------------------
 * (GameObjectExtensions.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */