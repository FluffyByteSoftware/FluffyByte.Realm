/*
 * (RealmServer.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@12:00:04 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net;

namespace FluffyByte.Realm.Networking.Server;

public class RealmServer(string name, IPAddress hostAddy, int hostPort, int maxPlayers)
{
    public string ServerName { get; private set; } = name;
    public IPAddress HostAddress { get; private set; } = hostAddy;
    public int HostPort { get; private set; } = hostPort;
    public int MaxPlayers { get; private set; } = maxPlayers;
    public DateTime LaunchedAt { get; init; } = DateTime.UtcNow;

    public TimeSpan Uptime => DateTime.UtcNow - LaunchedAt;
}

/*
 *------------------------------------------------------------
 * (RealmServer.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */