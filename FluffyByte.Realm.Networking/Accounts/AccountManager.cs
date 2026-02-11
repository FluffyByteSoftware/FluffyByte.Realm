/*
 * (AccountManager.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@10:19:32 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */
using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.ServerCore.Events;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Networking.Accounts;

public static class AccountManager
{
    public static void Initialize()
    {
        EventManager.Subscribe<PeerConnectedEvent>(OnPeerConnected);
        EventManager.Subscribe<PeerDisconnectedEvent>(OnPeerDisconnected);
        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Subscribe<NetworkReceiveEvent>(OnNetworkReceive);
    }

    private static void OnPeerConnected(PeerConnectedEvent e)
    {
        var client = RealmClientManager.GetClientByPeer(e.Peer);

        if (client == null)
        {
            Log.Error($"[AccountManager]: Client not found for peer {e.Peer.Address}.");
            return;
        }
        Log.Debug($"[AccountManager]: Found Client: {client.Peer.Address}, {client.Peer.Id}");
        
        SendAuthenticationRequest(client);
    }

    private static void OnPeerDisconnected(PeerDisconnectedEvent e)
    {
        Log.Debug($"[AccountManager]: Peer disconnected: {e.Peer.Id} from {e.Peer.Address}");
    }

    private static void SendAuthenticationRequest(RealmClient client)
    {
        Log.Debug(
            $"[AccountManager]: Requesting authentication from client: {client.Peer.Id} from {client.Peer.Address}");
        
        var packet = new PreAuthGeneralPacket()
        {
            Header = 0,
            Message = "Please provide your username and password."
        };

        var ndw = new NetDataWriter();
        ndw.Put((byte)PacketType.PreAuthGeneral);
        ndw.Put(packet);
        
        client.SendPacket(ndw);
    }

    private static void OnSystemShutdown(SystemShutdownEvent e)
    {
        EventManager.Unsubscribe<PeerConnectedEvent>(OnPeerConnected);
        EventManager.Unsubscribe<PeerDisconnectedEvent>(OnPeerDisconnected);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Unsubscribe<NetworkReceiveEvent>(OnNetworkReceive);
    }

    private static void OnNetworkReceive(NetworkReceiveEvent e)
    {
        try
        {
            var packetType = (PacketType)e.Reader.GetByte();

            if (packetType == PacketType.LoginRequest)
            {
                var client = RealmClientManager.GetClientByPeer(e.Peer);

                if (client == null)
                {
                    Log.Error($"[AccountManager]: Client not found for peer");
                    return;
                }

                var packet = new LoginPacket();
                packet.Deserialize(e.Reader);

                HandleLoginRequest(client, packet);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[AccountManager]: Error Processing Packet", ex);
        }
    }

    private static void HandleLoginRequest(RealmClient client, LoginPacket packet)
    {
        Log.Debug($"[AccountManager]: Login request from {client.ClientId}: Username={packet.Username}");

        client.PendingAccountName = packet.Username;
        client.PendingHashedPassword = packet.PasswordHash;
        
    }
}

/*
 *------------------------------------------------------------
 * (AccountManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */