/*
 * (RealmManagerEvents.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@9:50:39 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Clients;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.Events;

public class OnPeerDisconnectedEvent : EventArgs
{
    public NetPeer Peer { get; set; } = null!;
    public DisconnectInfo DisconnectInfo { get; set; }
}

public class OnNetworkReceivedEvent : EventArgs
{
    public NetPeer Peer { get; set; } = null!;
    public NetPacketReader Reader { get; set; } = null!;
    public byte Channel { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
}

public class OnAuthenticationSuccessEvent : EventArgs
{
    public RealmClient Client { get; set; } = null!;
}


/*
 *------------------------------------------------------------
 * (RealmManagerEvents.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */