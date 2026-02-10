/*
 * (LiteNetLibEvents.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@8:36:22 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.ServerCore.Events;

public class PeerConnectedEvent : EventArgs
{
    public NetPeer Peer { get; set; }

    public PeerConnectedEvent(NetPeer peer)
    {
        Peer = peer;
    }
}

public class PeerDisconnectedEvent : EventArgs
{
    public NetPeer Peer { get; set; }
    public DisconnectInfo DisconnectInfo { get; set; }

    public PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Peer = peer;
        DisconnectInfo = disconnectInfo;
    }
}

public class NetworkReceiveEvent : EventArgs
{
    public NetPeer Peer { get; set; }
    public NetPacketReader Reader { get; set; }
    public byte ChannelNumber { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }

    public NetworkReceiveEvent(NetPeer peer,
        NetPacketReader reader,
        byte channelNumber,
        DeliveryMethod deliveryMethod)
    {
        Peer = peer;
        Reader = reader;
        ChannelNumber = channelNumber;
        DeliveryMethod = deliveryMethod;
    }
}

public class NetworkReceiveUnconnectedEvent : EventArgs
{
    public IPEndPoint RemoteEndPoint { get; set; }
    public NetPacketReader Reader { get; set; }
    public UnconnectedMessageType MessageType { get; set; }

    public NetworkReceiveUnconnectedEvent(IPEndPoint remoteEndPoint,
        NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        RemoteEndPoint = remoteEndPoint;
        Reader = reader;
        MessageType = messageType;
    }
}

public class NetworkErrorEvent : EventArgs
{
    public IPEndPoint EndPoint { get; set; }
    public SocketError SocketError { get; set; }

    public NetworkErrorEvent(IPEndPoint endPoint, SocketError socketError)
    {
        EndPoint = endPoint;
        SocketError = socketError;
    }
}

public class NetworkLatencyUpdateEvent : EventArgs
{
    public NetPeer Peer { get; set; }
    public int Latency { get; set; }

    public NetworkLatencyUpdateEvent(NetPeer peer, int latency)
    {
        Peer = peer;
        Latency = latency;
    }
}

public class ConnectionRequestReceivedEvent : EventArgs
{
    public IPEndPoint RemoteEndPoint { get; set; }
    public bool Accepted { get; set; }
    public string? RejectionReason { get; set; }
    
    public ConnectionRequestReceivedEvent(IPEndPoint remoteEndPoint, bool accepted, string? rejectionReason)
    {
        RemoteEndPoint = remoteEndPoint;
        Accepted = accepted;
        RejectionReason = rejectionReason;
    }
}

/*
 *------------------------------------------------------------
 * (LiteNetLibEvents.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */