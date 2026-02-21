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

/// <summary>
/// Provides functionality to manage the lifecycle of a RealmServer instance, including initialization
/// and event subscriptions for system startup and shutdown.
/// This class acts as the central control point for the Realm server's runtime behavior.
/// </summary>
/// <remarks>
/// The RealmManager is responsible for creating and maintaining the active RealmServer instance,
/// initializing related services, and handling system lifecycle events.
/// </remarks>
public static class RealmManager
{
    /// <summary>
    /// Gets or sets the currently active <see cref="RealmServer"/> instance managed by the <c>RealmManager</c>.
    /// </summary>
    /// <remarks>
    /// This property holds the instance of the <see cref="RealmServer"/> that is currently being managed and operated.
    /// It can be assigned during the initialization process or manipulated as part of lifecycle management tasks.
    /// When <c>null</c>, it indicates that no RealmServer is actively managed.
    /// Changes to this property may directly affect server operations such as event subscriptions or network behavior.
    /// </remarks>
    public static RealmServer? ActiveRealm { get; set; }

    private const string ClockName = "NetworkPoller";
    private const int HeartbeatIntervalMs = 10;
    private static Clock? _clock;
    
    private static EventBasedNetListener? _listener;
    private static NetManager? _netManager;
    
    private static bool _isInitialized;
    
    #region Life Cycle

    /// <summary>
    /// Initializes the RealmManager by setting up the primary realm server, subscribing to system events,
    /// and configuring the internal clock for heartbeat-based logic.
    /// </summary>
    /// <remarks>
    /// This method performs the necessary setup for the RealmManager, including:
    /// - Creating and assigning an instance of the `RealmServer` to manage the realm.
    /// - Subscribing to system-level startup and shutdown events.
    /// - Registering a clock with a heartbeat interval to manage periodic tasks.
    /// Once initialized, the method ensures that it cannot be re-invoked.
    /// </remarks>
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
        
        
        _isInitialized = true;
        
        Log.Info($"[RealmManager]: Initialized.");

        _clock = ClockManager.RegisterClock(ClockName, HeartbeatIntervalMs);
        _clock.OnTick += OnTick;
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
        if (_listener == null || _netManager == null || !_isInitialized || _clock == null)
            return;
        
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
        
        _clock.OnTick -= OnTick;
        _clock = null;
        
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

    private static void OnTick()
    {
        if (_clock == null || !_isInitialized || _netManager == null)
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