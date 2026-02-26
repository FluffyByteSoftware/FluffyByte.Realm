/*
 * (SpawnRequest.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 26, 2026@10:46:07 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;

namespace FluffyByte.Realm.Game.Brains.Helpers;

public class SpawnRequest
{
    public required GameObject GameObjectToSpawn { get; init; }
    public required int GlobalX { get; init; }
    public required int GlobalZ { get; init; }
    public int SearchRadius { get; init; } = 5;
    public TaskCompletionSource<RealmTile?> Completion { get; } = new();
}

/*
 *------------------------------------------------------------
 * (SpawnRequest.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */