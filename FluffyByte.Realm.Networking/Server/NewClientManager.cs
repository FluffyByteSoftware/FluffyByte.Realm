/*
 * (NewClientManager.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@8:42:37 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Clients;

namespace FluffyByte.Realm.Networking.Server;

public static class NewClientManager
{
    public static void WelcomeNewClient(RealmClient client)
    {
        ClientManager.AddRealmClient(client);
    }
}

/*
 *------------------------------------------------------------
 * (NewClientManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */