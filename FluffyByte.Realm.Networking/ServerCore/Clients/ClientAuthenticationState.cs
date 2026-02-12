/*
 * (ClientAuthenticationState.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@8:06:51 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Networking.ServerCore.Clients;

public enum ClientAuthenticationState
{
    Rejected = -1,
    Fresh = 0,
    Authenticated = 1,
    Pending = 2
}

/*
 *------------------------------------------------------------
 * (ClientAuthenticationState.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */