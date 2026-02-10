/*
 * (AccountManager.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@10:19:32 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.ServerCore.Events;
using FluffyByte.Realm.Tools.Broadcasting;

namespace FluffyByte.Realm.Networking.Accounts;

public static class AccountManager
{
    public static void Initialize()
    {
        EventManager.Subscribe<PeerConnectedEvent>(OnPeerConnected);
    }

    private static void OnPeerConnected(PeerConnectedEvent e)
    {
        var rc = new RealmClient(e.Peer, e.Peer.Id);
    }
}

/*
 *------------------------------------------------------------
 * (AccountManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */