/*
 * (IGameObjectCommand.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@10:32:49 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Primitives;

public interface IGameObjectCommand
{
    void Execute(GameObject target);
}

/*
 *------------------------------------------------------------
 * (IGameObjectCommand.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */