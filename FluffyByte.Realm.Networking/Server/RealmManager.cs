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
using FluffyByte.Realm.Tools.Heartbeats;
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

        var addy = IPAddress.Parse("10.0.0.84");
        
        ActiveRealm = new RealmServer("Taerin's Whisper",
            addy,
            9997, 10);

        EventManager.Subscribe<SystemStartupEvent>(OnStart);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
        EventManager.Subscribe<TickEvent>(OnTick);
        
        _isInitialized = true;
        
        Log.Info($"[RealmManager]: Initialized.");

        ClockManager.RegisterClock("NetworkPoller", 15);
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
        
        _netManager.Start(ActiveRealm.HostAddress, IPAddress.IPv6Any, ActiveRealm.HostPort);

        ClockManager.StartClock("NetworkPoller");
        
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
        EventManager.Unsubscribe<TickEvent>(OnTick);
        
        _isInitialized = false;
        
        Log.Info($"[RealmManager]: Shutdown.");
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
            return;
        }
        
        Log.Debug($"[RealmManager]: Connection Request from {request.RemoteEndPoint} accepted.");
        
        request.AcceptIfKey("FluffyByte");
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

    private static void OnTick(TickEvent e)
    {
        if (e.ClockName != "NetworkPoller" || _netManager == null || !_isInitialized)
            return;

        _netManager.PollEvents();
    }
}

/*
 *------------------------------------------------------------
 * (RealmManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */