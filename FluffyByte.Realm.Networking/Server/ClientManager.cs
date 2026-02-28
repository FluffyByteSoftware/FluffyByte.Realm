/*
 * (ClientManager.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@1:15:12 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Server;

public static class ClientManager
{
    private static bool _isInitialized;
    public static Dictionary<int, RealmClient> Clients { get; private set; } = [];

    #region Life Cycle
    public static void Initialize()
    {
        if (_isInitialized) return;
        
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);

        EventManager.Subscribe<OnPeerDisconnectedEvent>(OnPeerDisconnected);
        
        _isInitialized = true;
    }
    
    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized) return;
        
        _isInitialized = false;

        foreach (var client in Clients.Values)
        {
            RemoveRealmClient(client);
        }

        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
        EventManager.Unsubscribe<OnPeerDisconnectedEvent>(OnPeerDisconnected);
        
        Clients.Clear();
    }
    #endregion Life Cycle
    
    public static void AddRealmClient(RealmClient client)
    {
        Clients.Add(client.Peer.Id, client);
    }

    public static void RemoveRealmClient(RealmClient client)
    {
        Clients.Remove(client.Peer.Id);
        
        if(client.IsConnected)
            client.Disconnect();
    }
    
    public static bool ContainsRealmClient(int id)
    {
        return Clients.ContainsKey(id);
    }

    public static RealmClient? TryGetClientById(int id, out RealmClient? client)
    {
        var c = Clients.TryGetValue(id, out client) ? client : null;
        return c;
    }

    private static void OnPeerDisconnected(OnPeerDisconnectedEvent e)
    {
        if (!Clients.TryGetValue(e.Peer.Id, out var client))
        {
            return;
        }

        Log.Info($"[ClientManager]: {client.Name} ({client.Address}) disconnected. Reason: {e.DisconnectInfo.Reason}");

        Clients.Remove(e.Peer.Id);
    }
}

/*
 *------------------------------------------------------------
 * (ClientManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */