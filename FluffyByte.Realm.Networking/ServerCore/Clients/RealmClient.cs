/*
 * (RealmClient.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@5:47:00 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Shared.PacketTypes;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Networking.ServerCore.Clients;

public class RealmClient(NetPeer peer)
{
    public NetPeer Peer { get; } = peer;
    
    private ConcurrentQueue<object> PacketQueue { get; set; }= [];

    public string Name { get; private set; } = $"(Client{peer.Id} from {peer.Address})";
    private ClientAuthenticationState AuthenticationState { get; set; } = ClientAuthenticationState.Fresh;
    public string Address => Peer.Address.ToString();
    public int Id => Peer.Id;

    private bool _isDisconnecting;
    
    public bool IsDisconnected => Peer.ConnectionState != ConnectionState.Connected 
                                  || _isDisconnecting 
                                  || AuthenticationState == ClientAuthenticationState.Rejected;

    public bool PendingInQueue => !PacketQueue.IsEmpty;
    
    public void EnqueuePacket<T>(T packet) where T : class => PacketQueue.Enqueue(packet);

    public void Send<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
    {
        if (IsDisconnected)
            return;

        var writer = new NetDataWriter();
        Peer.Send(writer, method);
    }

    public void Disconnect(DisconnectReason reason)
    {
        if (_isDisconnecting) return;

        _isDisconnecting = true;
        AuthenticationState = ClientAuthenticationState.Rejected;

        var disconnectPacket = new DisconnectPacket()
        {
            DisconnectReason = reason
        };

        if (Peer.ConnectionState == ConnectionState.Connected)
        {
            Send(disconnectPacket);
        }

        Peer.Disconnect();
    }

    public object? PopQueueNext() => PacketQueue.TryDequeue(out var packet) ? packet : null;
}

/*
 *------------------------------------------------------------
 * (RealmClient.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */