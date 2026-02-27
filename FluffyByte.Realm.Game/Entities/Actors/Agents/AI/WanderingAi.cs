/*
 * (WanderingAI.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 26, 2026@5:10:51 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Diagnostics;
using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Entities.Actors.Agents.AI;

public class WanderingAi(int moveIntervalTicks = 30) : GameObjectComponent
{
    public override TickType TickType => TickType.Normal;

    private int _ticksSinceLastMove = 0;

    public override void Tick()
    {
        if (Owner == null)
        {
            Log.Warn("Wandering Ai has no owner!");
            return;
        }

        _ticksSinceLastMove++;

        if (_ticksSinceLastMove < moveIntervalTicks)
            return;

        _ticksSinceLastMove = 0;

        var transform = Owner.GetTransform();

        if (transform?.Tile == null) return;

        var dx = Random.Shared.Next(-1, 2);
        var dz = Random.Shared.Next(-1, 2);

        if (dx == 0 && dz == 0)
            return;

        var targetX = transform.Tile.GlobalX + dx;
        var targetZ = transform.Tile.GlobalZ + dz;

        Log.Debug($"Wandering AI is moving!");
        
        GameDirector.RequestMove(Owner, targetX, targetZ);
    }
}

/*
 *------------------------------------------------------------
 * (WanderingAI.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */