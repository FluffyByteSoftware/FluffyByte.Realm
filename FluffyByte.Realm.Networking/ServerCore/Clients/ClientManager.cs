/*
 * (ClientManager.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@7:04:22 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.ServerCore.Clients;

/// <summary>
/// The ClientManager class provides functionality to manage and maintain a list of
/// clients connected to the system. It is responsible for initializing the client
/// management system, adding new clients, and handling internal state related to clients.
/// </summary>
public static class ClientManager
{
    /// <summary>
    /// Represents the collection of connected clients managed by the system.
    /// This property provides access to the list of currently active clients
    /// within the server's management system. It is initialized and maintained
    /// by the ClientManager class and allows read-only access to external consumers.
    /// </summary>
    private static List<RealmClient> Clients { get; set; } = [];

    private static readonly Dictionary<NetPeer, RealmClient> PeerMap = [];
    
    private static bool _isInitialized;

    #region Client Management
    /// <summary>
    /// Adds a new client to the client management system.
    /// If the specified client is not already in the list of clients, it will be added.
    /// </summary>
    /// <param name="client">The client instance to be added to the system.</param>
    public static void AddClient(RealmClient client)
    {
        if (!Clients.Contains(client))
        {
            Clients.Add(client);
            PeerMap.Add(client.Peer, client);
        }
    }

    public static void RemoveClient(RealmClient client)
    {
        if (!client.IsDisconnected)
            client.Disconnect(DisconnectReason.ConnectionRejected);
     

        Clients.Remove(client);
        PeerMap.Remove(client.Peer);
    }

    public static RealmClient? GetClientByPeer(NetPeer peer)
    {
        return PeerMap.GetValueOrDefault(peer);
    }
    
    /// <summary>
    /// Gets the total number of clients currently connected to the system.
    /// </summary>
    public static int ClientCount => Clients.Count;
    #endregion Client Management
    
    #region Life Cycle Management
    /// <summary>
    /// Initializes the client management system.
    /// This method sets up the internal state required for managing clients, such as clearing
    /// the list of clients and subscribing to necessary system events. It ensures the system
    /// is initialized only once.
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized)
            return;

        Clients.Clear();
        
        _isInitialized = true;
        EventManager.Subscribe<SystemShutdownEvent>(Shutdown);
    }

    /// <summary>
    /// Handles system shutdown by unsubscribing the Shutdown method from the SystemShutdownEvent.
    /// Ensures that the client manager properly releases its resources and detaches from the event system
    /// during shutdown.
    /// </summary>
    /// <param name="e">The event data associated with the system shutdown.</param>
    private static void Shutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized) return;

        foreach (var client in Clients.ToList())
        {
            client.Disconnect(DisconnectReason.RemoteConnectionClose);
        }
        
        Clients.Clear();
        
        _isInitialized = false;
        
        EventManager.Unsubscribe<SystemShutdownEvent>(Shutdown);
    }
    
    #endregion Life Cycle Management
}

/*
 *------------------------------------------------------------
 * (ClientManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */