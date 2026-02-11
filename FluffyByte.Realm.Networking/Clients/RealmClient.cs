/*
 * (RealmClient.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@1:24:07 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Networking.Clients;

public class RealmClient(NetPeer peer, int clientId)
{
    public NetPeer Peer { get; private set; } = peer;
    public int ClientId { get; private set; } = clientId;

    public string? AccountName { get; private set; }
    
    public string? PendingAccountName { get; set; } = string.Empty;
    public byte[]? PendingHashedPassword { get; set; }

    public bool IsAuthenticated { get; private set; } = false;

    public DateTime ConnectedAt { get; private set; } = DateTime.UtcNow;
    public int Ping => Peer.Ping;

    public bool IsConnected => Peer.ConnectionState == ConnectionState.Connected;

    public void SendPacket(byte[] data, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
    {
        try
        {
            var ndw = new NetDataWriter();
            ndw.Put(data);
            Peer.Send(ndw, deliveryMethod);
        }
        catch (Exception ex)
        {
            Log.Error($"Client: {ClientId}", ex);
        }
    }

    public void SendPacket(NetDataWriter writer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
    {
        try
        {
            Peer.Send(writer, deliveryMethod);
        }
        catch (Exception ex)
        {
            Log.Error($"Client: {ClientId}", ex);
        }
    }

    public void Disconnect(string reason = "")
    {
        try
        {
            if (!string.IsNullOrEmpty(reason))
            {
                var writer = new NetDataWriter();
                writer.Put(reason);
                Peer.Disconnect(writer);
            }
            else
            {
                Peer.Disconnect();
            }

            Log.Info($"[RealmClient]: Disconnected client: {ClientId} from {Peer.Address}: {reason}");
        }
        catch (Exception ex)
        {
            Log.Error($"Error disconnecting client {ClientId} from {Peer.Address}", ex);
        }
    }

    public double GetConnectionDuration()
    {
        return (DateTime.UtcNow - ConnectedAt).TotalSeconds;
    }

    public override string ToString()
    {
        return $"RealmClient[{ClientId}] - {AccountName ?? "Unauthenticated"} @ {Peer.Address} (Ping: {Ping} ms)";
    }

    public void SetAccountName(string accountName)
    {
        AccountName = accountName;
    }
}

/*
 *------------------------------------------------------------
 * (RealmClient.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */