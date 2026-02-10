/*
 * (SentinelListener.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@1:52:25 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net;
using FluffyByte.Realm.Networking.ServerCore.Events;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.ServerCore;

public class SentinelListener(RealmServer config) : INetEventListener
{
    private readonly RealmServer _serverConfig = config;
    
    public void OnPeerConnected(NetPeer peer)
    {
        EventManager.Publish(new PeerConnectedEvent(peer));
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        EventManager.Publish(new PeerDisconnectedEvent(peer, disconnectInfo));
    }

    public void OnNetworkError(IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
    {
        EventManager.Publish(new NetworkErrorEvent(endPoint, socketError));
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, 
        DeliveryMethod deliveryMethod)
    {
        try
        {
            EventManager.Publish(new NetworkReceiveEvent(peer, reader, channelNumber, deliveryMethod));
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, 
        UnconnectedMessageType messageType)
    {
        EventManager.Publish(new NetworkReceiveUnconnectedEvent(remoteEndPoint, reader, messageType));
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        EventManager.Publish(new NetworkLatencyUpdateEvent(peer, latency));
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        try
        {
            if (Sentinel.GetConnectedPeerCount() >= _serverConfig.MaxPlayers)
            {
                Log.Debug($"[Sentinel] Connection rejected - server full.");
                request.Reject();
                return;
            }

            if (!string.IsNullOrEmpty(_serverConfig.ConnectionKey))
            {
                var requestData = request.Data;

                if (requestData.GetString() != _serverConfig.ConnectionKey)
                {
                    Log.Debug($"[Sentinel] Connection rejected - invalid key.");
                    request.Reject();

                    EventManager.Publish(new ConnectionRequestReceivedEvent(
                        request.RemoteEndPoint, accepted: false, rejectionReason: "invalid key"));

                    request.Reject();
                    return;
                }
            }

            Log.Debug($"[Sentinel] Connection accepted - {request.RemoteEndPoint}");
            EventManager.Publish(new ConnectionRequestReceivedEvent(
                request.RemoteEndPoint,
                accepted: true,
                rejectionReason: null));

            request.Accept();
        }
        catch (Exception ex)
        {
            Log.Error($"[Senitnel]: Error handling connection request from {request.RemoteEndPoint}", ex);
            request.Reject();
        }
    }
    
}

/*
 *------------------------------------------------------------
 * (SentinelListener.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */