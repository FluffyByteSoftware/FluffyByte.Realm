/*
 * (PlayerProfile.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@10:56:12 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Clients;

namespace FluffyByte.Realm.Game.Entities.Actors.Players;

public class PlayerProfile : ActorTemplate
{
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/*
 *------------------------------------------------------------
 * (PlayerProfile.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */