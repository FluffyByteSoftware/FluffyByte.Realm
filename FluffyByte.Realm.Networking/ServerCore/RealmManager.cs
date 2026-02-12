/*
 * (RealmManager.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@5:40:30 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.LoginServer;
using FluffyByte.Realm.Networking.ServerCore.Clients;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using LiteNetLib;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.ServerCore;

/// <summary>
/// Provides high-level server management functionality for handling network communication and server lifecycle.
/// Formerly known as Sentinel - this process is what wraps around the NetManager from LiteNetLib.
/// This process will help us manage the server's network operations, and route information to other necessary managers.
/// </summary>
public static class RealmManager
{
    #region Local Properties
    private static NetManager? _netManager;
    private static EventBasedNetListener? _listener;
    private static RealmServer? _realmServer;

    private static bool _isInitialized;

    /// <summary>
    /// Provides access to the current instance of the <see cref="RealmServer"/> being managed by the
    /// <c>RealmManager</c>. This property represents the active server configuration and state.
    /// </summary>
    /// <remarks>
    /// The <c>Server</c> property is null until the <c>Initialize</c> method is called with a valid
    /// <see cref="RealmServer"/> instance. Once initialized, this property can be used to retrieve
    /// details about the server, such as its name, maximum player count, IP address, and port.
    /// </remarks>
    public static RealmServer? Server => _realmServer;

    #endregion Local Properties
    
    /// <summary>
    /// Initializes the RealmManager with the specified server instance and sets up necessary event listeners.
    /// </summary>
    /// <param name="server">The instance of <see cref="RealmServer"/> to use for managing the server configuration
    /// and state.</param>
    public static void Initialize(RealmServer server)
    {
        if (_isInitialized || _realmServer != null)
            return;

        _realmServer = server;

        _listener = new EventBasedNetListener();
        _netManager = new NetManager(_listener);

        _listener.PeerConnectedEvent += OnPeerConnected;
        _listener.PeerDisconnectedEvent += OnPeerDisconnected;
        _listener.NetworkReceiveEvent += OnNetworkReceive;
        _listener.ConnectionRequestEvent += OnConnectionRequest;

        _isInitialized = true;
        Log.Info($"[RealmManager]: Initialized - {_realmServer.ServerName}");
        
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
    }

    /// <summary>
    /// Starts the network server using the current configuration of the RealmServer instance.
    /// </summary>
    /// <returns>
    /// Returns true if the server starts successfully; otherwise, false if initialization has not occurred
    /// or the necessary parts are null.
    /// </returns>
    private static void OnStart()
    {
        if (!_isInitialized || _netManager == null || _realmServer == null)
            return;

        if (_netManager.Start(_realmServer.Address, null, _realmServer.Port))
        {
            Log.Info($"[RealmManager]: {_realmServer.ServerName} started!");
            Log.Info($"[RealmManager]: Listening on {_realmServer.Address}:{_realmServer.Port}");
        }
    }

    /// <summary>
    /// Processes all incoming and outgoing network events for the current server instance.
    /// This method should be called periodically in the main server loop to handle network communication.
    /// </summary>
    public static void Poll() => _netManager?.PollEvents();
    
    #region LiteNetLib Callbacks

    /// <summary>
    /// Handles incoming connection requests from clients attempting to connect to the server.
    /// </summary>
    /// <param name="request">The connection request containing details about the client attempting to connect.</param>
    private static void OnConnectionRequest(ConnectionRequest request)
    {
        if (_realmServer == null)
        {
            request.Reject();
            return;
        }

        if (_netManager!.ConnectedPeersCount >= _realmServer.MaxPlayers)
        {
            Log.Warn($"[RealmManager]: Server is full. Rejecting connection from {request.RemoteEndPoint}");
            request.Reject();
            return;
        }

        request.Accept();
        Log.Info($"[RealmManager]: Accepted connection from {request.RemoteEndPoint}");
    }

    /// <summary>
    /// Handles the event when a peer successfully connects to the server.
    /// </summary>
    /// <param name="peer">The network peer that has connected to the server.</param>
    private static void OnPeerConnected(NetPeer peer)
    {
        Log.Info($"[RealmManager]: Peer Connected - {peer.Address}:{peer.Port}");

        var client = new RealmClient(peer);

        ClientManager.AddClient(client);

        LoginManager.WelcomeNewClient(client);
    }

    /// <summary>
    /// Handles the event when a peer disconnects from the server.
    /// </summary>
    /// <param name="peer">The network peer that has disconnected.</param>
    /// <param name="disconnectInfo">Information about the disconnection, including the reason and additional
    /// details.</param>
    private static void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Log.Info($"[RealmManager]: Peer Disconnected - {peer.Address}:{peer.Port}");

        var client = ClientManager.GetClientByPeer(peer);

        if (client != null)
        {
            ClientManager.RemoveClient(client);
        }
    }

    /// <summary>
    /// Handles incoming network data received from a peer.
    /// </summary>
    /// <param name="peer">The network peer that sent the data.</param>
    /// <param name="reader">The packet reader containing the received data.</param>
    /// <param name="channel">The channel number on which the data was received.</param>
    /// <param name="deliveryMethod">The delivery method used for this data transfer.</param>
    private static void OnNetworkReceive(NetPeer peer,
        NetPacketReader reader,
        byte channel,
        DeliveryMethod deliveryMethod)
    {
        var client = ClientManager.GetClientByPeer(peer);
        
        if (client == null)
        {
            reader.Recycle();
            return;
        }
        
        PacketManager.Route(client, reader);
        
        reader.Recycle();
    }
    #endregion LiteNetLib Callbacks

    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized) return;

        _realmServer = null;
        
        _netManager?.Stop();
        _netManager = null;
        
        _isInitialized = false;
    }
}

/*
 *------------------------------------------------------------
 * (RealmManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */