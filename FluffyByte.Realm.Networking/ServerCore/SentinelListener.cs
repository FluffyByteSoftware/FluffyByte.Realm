/*
 * (SentinelListener.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@1:52:25 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.ServerCore;

public class SentinelListener : INetEventListener
{
    private readonly RealmServer _serverConfig;
    
    public void OnPeerConnected(NetPeer peer)
    {
        Log.Debug($"Peer connected: {peer.Address}");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Log.Debug($"Peer disconnected: {peer.Address} - Reason: {disconnectInfo.Reason}");
    }

    public void OnNetworkError(IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
    {
        Log.Debug($"Network error from {endPoint}: {socketError}");
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, 
        DeliveryMethod deliveryMethod)
    {
        try
        {
            Log.Debug($"[Sentinel] Received {reader.AvailableBytes} bytes from {peer.Address}");

        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, 
        UnconnectedMessageType messageType)
    {
        Log.Debug($"[Sentinel] Unconnected message from {remoteEndPoint}: Type: {messageType}");
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        // Handle latency updates here
        // TODO
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        // TODO: Validate connection key

        if (_serverConfig != null && Sentinel.GetConnectedPeerCount() >= _serverConfig.MaxPlayers)
        {
            Log.Debug($"[Sentinel] Connection rejected - server full.");
            request.Reject();
            return;
        }

        Log.Debug($"[Sentinel] Connection accepted - {request.RemoteEndPoint}");
        request.Accept();
    }
}

/*
 *------------------------------------------------------------
 * (SentinelListener.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */