/*
 * (RealmClientManager.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@8:13:42 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Networking.ServerCore.Events;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.Clients;

public static class RealmClientManager
{
    private static readonly ConcurrentDictionary<NetPeer, RealmClient> ClientsByPeer = [];
    private static readonly ConcurrentDictionary<int, RealmClient> ClientsById = [];
    private static readonly ConcurrentDictionary<string, RealmClient> ClientsByUsername = [];

    private static readonly Lock Lock = new Lock();
    private static int _nextClientId = 1;
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
        {
            Console.WriteLine($"[RealmClientManager]: Already initialized.");
            return;
        }

        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Subscribe<PeerConnectedEvent>(OnPeerConnected);
        EventManager.Subscribe<PeerDisconnectedEvent>(OnPeerDisconnected);
        
        _initialized = true;
        Log.Debug($"[RealmClientManager]: Initialized.");
    }

    public static RealmClient RegisterClient(NetPeer peer)
    {
        lock (Lock)
        {
            if (ClientsByPeer.TryGetValue(peer, out var value))
            {
                Log.Debug($"[RealmClientManager]: Client already registered: {peer.Address}");
                return value;
            }

            var client = new RealmClient(peer, _nextClientId++);

            ClientsByPeer[peer] = client;
            ClientsById[client.ClientId] = client;

            Log.Debug($"[RealmClientManager]: Registered client {client.ClientId} from {client.Peer.Address}");

            return client;
        }
    }
    
    public static void UnregisterClient(NetPeer peer)
    {
        lock (Lock)
        {
            if (ClientsByPeer.TryRemove(peer, out var client))
            {
                ClientsById.TryRemove(client.ClientId, out _);

                if (!string.IsNullOrEmpty(client.AccountName))
                {
                    ClientsByUsername.TryRemove(client.AccountName, out _);
                    
                }

                Log.Debug($"[RealmClientManager]: Unregistered client: {client.ClientId} from {client.Peer.Address}");
            }
        }
    }

    public static void RegisterUsername(RealmClient client, string username)
    {
        lock (Lock)
        {
            if (string.IsNullOrEmpty(username))
                return;

            if (!string.IsNullOrEmpty(client.AccountName))
                ClientsByUsername.TryRemove(client.AccountName, out _);

            client.SetAccountName(username);
            ClientsByUsername[username] = client;

            Log.Debug($"[RealmClientManager]: Client {client.ClientId} authenticated as '{username}'");
        }
    }

    public static RealmClient? GetClientByPeer(NetPeer peer)
    {
        ClientsByPeer.TryGetValue(peer, out var client);
        
        return client;
    }

    public static RealmClient? GetClientById(int clientId)
    {
        ClientsById.TryGetValue(clientId, out var client);
        
        return client;
    }

    public static RealmClient? GetClientByUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return null;

        ClientsByUsername.TryGetValue(username, out var client);
        return client;
    }

    public static IReadOnlyCollection<RealmClient> GetAllClients()
    {
        return ClientsByPeer.Values.ToList();
    }

    public static int GetClientCount()
    {
        return ClientsByPeer.Count;
    }

    public static int GetAuthenticatedClientCount()
    {
        return ClientsByUsername.Count;
    }

    public static bool IsAccountNameConnect(string username)
    {
        username = username.ToLower();
        
        return ClientsByUsername.ContainsKey(username);
    }

    public static void DisconnectAllClients(string reason = "Server Kicking All")
    {
        lock (Lock)
        {
            foreach (var client in ClientsByPeer.Values)
            {
                client.Disconnect(reason);
            }

            Log.Debug($"[RealmClientManager]: Disconnected all clients; {reason}");
        }
    }

    private static void Clear()
    {
        lock (Lock)
        {
            ClientsByPeer.Clear();
            ClientsById.Clear();
            ClientsByUsername.Clear();
            _nextClientId = 1;

            Log.Debug($"[RealmClientManager]: Cleared all client registration.");
        }
    }

    private static void OnSystemShutdown(SystemShutdownEvent e)
    {
        Log.Debug($"[RealmClientManager]: System shutdown detected.");
        Shutdown();
    }

    public static void Shutdown()
    {
        if (!_initialized)
            return;

        DisconnectAllClients();
        Clear();

        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Unsubscribe<PeerConnectedEvent>(OnPeerConnected);
        EventManager.Unsubscribe<PeerDisconnectedEvent>(OnPeerDisconnected);

        _initialized = false;
        Log.Debug($"[RealmClientManager]: Shutdown task completed.");
    }

    private static void OnPeerConnected(PeerConnectedEvent e)
    {
        RegisterClient(e.Peer);
    }

    private static void OnPeerDisconnected(PeerDisconnectedEvent e)
    {
        UnregisterClient(e.Peer);
    }
}


/*
 *------------------------------------------------------------
 * (RealmClientManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */