/*
 * (RealmServer.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@1:44:32 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Networking.ServerCore;

public class RealmServer
{
    public string ServerName { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public int MaxPlayers { get; set; }
    public string ConnectionKey { get; set; }

    public RealmServer(string name, string ip, int port, int maxPlayers, string key)
    {
        ServerName = name;
        IpAddress = ip;
        Port = port;
        MaxPlayers = maxPlayers;
        ConnectionKey = key;
    }
}

/*
 *------------------------------------------------------------
 * (RealmServer.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */