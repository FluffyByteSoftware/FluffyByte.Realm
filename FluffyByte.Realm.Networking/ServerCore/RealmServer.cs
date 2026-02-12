/*
 * (RealmServer.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@2:12:39 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Networking.ServerCore;

public class RealmServer(
    string servername,
    int maxplayers,
    string ipv4Address,
    int port)
{
    public string ServerName { get; set; } = servername;
    public int MaxPlayers { get; set; } = maxplayers;
    public string Address { get; set; } = ipv4Address;
    public int Port { get; set; } = port;

}

/*
 *------------------------------------------------------------
 * (RealmServer.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */