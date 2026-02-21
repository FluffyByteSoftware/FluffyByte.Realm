/*
 * (IGameObjectOwner.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 19, 2026@4:40:18 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Primitives;

public interface IGameObjectOwner
{
    Guid Id { get; }
    void AddGameObject(GameObject obj);
    void RemoveGameObject(GameObject obj);
}

/*
 *------------------------------------------------------------
 * (IGameObjectOwner.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */