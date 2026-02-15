/*
 * (RealmManager.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@12:03:26 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net;
using System.Net.Sockets;
using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.Server;

public static class RealmManager
{
    public static RealmServer? ActiveRealm { get; set; }

    private static EventBasedNetListener? _listener;
    private static NetManager? _netManager;
    
    private static bool _isInitialized;
    
    #region Life Cycle
    public static void Initialize()
    {
        if (_isInitialized)
            return;
        
        ActiveRealm = new RealmServer("Taerin's Whisper",
            IPAddress.Parse("10.0.0.84"),
            9997, 10);

        EventManager.Subscribe<SystemStartupEvent>(OnStart);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
        
        _isInitialized = true;
        
        Log.Info($"[RealmManager]: Initialized.");
    }
    
    private static void OnStart(SystemStartupEvent e)
    {
        if (ActiveRealm == null)
            return;
        
        _listener = new EventBasedNetListener();
        _netManager = new NetManager(_listener);

        _listener.ConnectionRequestEvent += OnConnectionRequest;
        _listener.PeerConnectedEvent += OnPeerConnected;
        _listener.PeerDisconnectedEvent += OnPeerDisconnected;
        _listener.NetworkReceiveEvent += OnNetworkReceive;
        _listener.NetworkErrorEvent += OnNetworkError;
        
        _netManager.Start(ActiveRealm.HostAddress, null, ActiveRealm.HostPort);

        Log.Info($"[RealmManager]: Started {ActiveRealm.ServerName} listening on " +
                 $"{ActiveRealm.HostAddress}:{ActiveRealm.HostPort}");
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (_listener == null || _netManager == null || !_isInitialized) return;
        
        ActiveRealm = null;
        
        _netManager.Stop();
        _netManager = null;
        
        _listener.ConnectionRequestEvent -= OnConnectionRequest;
        _listener.PeerConnectedEvent -= OnPeerConnected;
        _listener.PeerDisconnectedEvent -= OnPeerDisconnected;
        _listener.NetworkReceiveEvent -= OnNetworkReceive;
        _listener.NetworkErrorEvent -= OnNetworkError;

        _listener = null;
        
        EventManager.Unsubscribe<SystemStartupEvent>(OnStart);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
    }
    #endregion Life Cycle
    
    #region Listener Events

    private static void OnConnectionRequest(ConnectionRequest request)
    {
        // TODO Check request.RemoteEndPoint against banlist.

        if (ClientManager.Clients.Count >= ActiveRealm?.MaxPlayers)
        {
            Log.Debug($"[RealmManager]: Connection Request from {request.RemoteEndPoint} denied. Max Players Reached.");
            request.Reject();    
        }
        
        request.Accept();
    }

    private static void OnPeerConnected(NetPeer peer)
    {
        var newClient = new RealmClient(peer);
        
        NewClientManager.WelcomeNewClient(newClient);
    }

    private static void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        EventManager.Publish(new OnPeerDisconnectedEvent()
        {
            Peer = peer,
            DisconnectInfo = disconnectInfo
        });
    }

    private static void OnNetworkReceive(NetPeer peer, 
        NetPacketReader reader, 
        byte channel, 
        DeliveryMethod deliveryMethod)
    {
        
        EventManager.Publish(new OnNetworkReceivedEvent
        {
            Peer = peer, 
            Reader = reader, 
            Channel = channel, 
            DeliveryMethod = deliveryMethod
        });
    }
    
    private static void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Log.Error($"[RealmManager]: Network Error at {endPoint}: {socketError}");
    }
    #endregion Listener Events

}

/*
 *------------------------------------------------------------
 * (RealmManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */