/*
 * (MoveRequest.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 26, 2026@10:39:34 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Brains.Helpers;

public class MoveRequest
{
    public GameObject GameObject { get; init; }
    public int TargetGlobalX { get; init; }
    public int TargetGlobalZ { get; init; }
    public TaskCompletionSource<RealmTile?> Completion { get; } = new();
}

/*
 *------------------------------------------------------------
 * (MoveRequest.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */