/*
 * (RealmClient.cs)
 *------------------------------------------------------------
 * Created - Friday, February 13, 2026@10:28:31 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using System.Net;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Networking.Clients;

public class RealmClient
{
    public NetPeer Peer { get; private set; }
    public string Name { get; private set; }

    public IPAddress Address => Peer.Address;
    public byte[]? AuthNonce { get; set; }
    
    private readonly ConcurrentDictionary<PacketType, ConcurrentQueue<IRealmPacket>> _packetQueues = [];

    public DateTime LoginTime { get; private set; } = DateTime.UtcNow;
    public DateTime LastResponseTime { get; private set; } = DateTime.UtcNow;
    public TimeSpan Uptime => DateTime.UtcNow - LoginTime;
    public TimeSpan IdleTime => DateTime.UtcNow - LastResponseTime;

    public RealmAccount? Account { get; private set; } 

    private bool _disconnecting;

    public RealmClient(NetPeer peer)
    {
        Peer = peer;
        Name = $"Client_{peer.Id}_{peer.Address}";
        
        Log.Info($"[{Name}]: has connected.");
    }
    public bool IsConnected
    {
        get
        {
            if (IdleTime <= TimeSpan.FromMinutes(30)
                && Peer.ConnectionState == ConnectionState.Connected
                && !_disconnecting) return true;
            
            Disconnect();
            return false;
        }
    }

    public void Enqueue(PacketType type, IRealmPacket packet)
    {
        LastResponseTime = DateTime.UtcNow;
        
        var queue = _packetQueues.GetOrAdd(type, _ => new ConcurrentQueue<IRealmPacket>());
        
        queue.Enqueue(packet);
    }

    public List<IRealmPacket> DrainQueue(PacketType type)
    {
        var drained = new List<IRealmPacket>();

        if (!_packetQueues.TryGetValue(type, out var queue))
            return drained;

        while (queue.TryDequeue(out var packet))
        {
            drained.Add(packet);
        }

        return drained;
    }

    public int PendingPacketCount(PacketType type) 
        => _packetQueues.TryGetValue(type, out var queue) ? queue.Count : 0;

    public int TotalPacketsPending
    {
        get
        {
            var count = 0;
            foreach (var queue in _packetQueues.Values)
                count += queue.Count;
            
            return count;
        }
    }

    public void SendPacket(PacketType type,
        IRealmPacket packet, 
        DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
    {
        var writer = new NetDataWriter();
        writer.Put((byte)type);
        packet.Serialize(writer);
        Peer.Send(writer, deliveryMethod);
    }

    public void SetAccount(RealmAccount account)
    {
        Account = account;
    }

    public void Disconnect()
    {
        if (_disconnecting)
            return;
        
        _disconnecting = true;
        
        Peer.Disconnect();

        _packetQueues.Clear();

        Log.Info($"[{Name}]: Has been disconnected.");
    }
}

/*
 *------------------------------------------------------------
 * (RealmClient.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */