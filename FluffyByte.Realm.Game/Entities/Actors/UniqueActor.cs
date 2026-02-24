/*
 * (UniqueActor.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@12:13:11 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Data.SqlTypes;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

namespace FluffyByte.Realm.Game.Entities.Actors;

public class UniqueActor(string name) : GameObject(name), IUniqueActor
{
    public bool IsUniqueActor => true;
}

/*
 *------------------------------------------------------------
 * (UniqueActor.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */