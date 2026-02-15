/*
 * (PacketManager.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@1:07:30 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.Server;

/// <summary>
/// A static class responsible for managing the handling of packets in the system.
/// It determines appropriate packet buckets based on packet types and facilitates
/// the initialization required to process network-based events effectively.
/// </summary>
public static class PacketManager
{
    /// <summary>
    /// Indicates whether the <c>PacketManager</c> has been successfully initialized.
    /// This variable ensures that initialization steps such as subscribing to events
    /// are performed only once during the application's lifecycle.
    /// </summary>
    private static bool _isInitialized;

    #region Life Cycle

    /// <summary>
    /// Initializes the PacketManager by subscribing to relevant events and ensuring
    /// the manager is prepared to handle network and system-related operations.
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized)
            return;

        EventManager.Subscribe<SystemStartupEvent>(OnStart);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
        EventManager.Subscribe<OnNetworkReceivedEvent>(OnNetworkReceived);
        
        _isInitialized = true;
    }

    /// <summary>
    /// Handles the startup process by performing necessary initialization
    /// tasks when the system starts up.
    /// </summary>
    /// <param name="e">The event data associated with the system startup.</param>
    private static void OnStart(SystemStartupEvent e)
    {
        if (!_isInitialized) return;
        
    }

    /// <summary>
    /// Handles the shutdown process by unsubscribing from relevant events
    /// and resetting the internal state for the packet manager.
    /// </summary>
    /// <param name="e">The event data associated with the system shutdown.</param>
    private static void OnShutdown(SystemShutdownEvent e)
    {
        if(!_isInitialized) return;
        
        _isInitialized = false;
        
        EventManager.Unsubscribe<SystemStartupEvent>(OnStart);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
        EventManager.Unsubscribe<OnNetworkReceivedEvent>(OnNetworkReceived);
    }
    #endregion Life Cycle
    
    #region Bucket Routing

    /// <summary>
    /// Determines the appropriate packet bucket based on the specified packet type.
    /// </summary>
    /// <param name="type">The packet type used to determine the corresponding packet bucket.</param>
    /// <returns>The packet bucket that corresponds to the specified packet type.</returns>
    public static PacketBucket GetBucket(PacketType type)
    {
        return (byte)type switch
        {
            < 50 => PacketBucket.Auth,
            < 70 => PacketBucket.Movement,
            < 100 => PacketBucket.Combat,
            _ => PacketBucket.General
        };
        
        
    }
    #endregion BucketRouting
    
    #region Packet Receiving

    /// <summary>
    /// Processes a network packet received from a specific peer and enqueues it for further handling if valid.
    /// </summary>
    /// <param name="e">The network received event containing the peer and packet data reader.</param>
    private static void OnNetworkReceived(OnNetworkReceivedEvent e)
    {
        if (!ClientManager.ContainsRealmClient(e.Peer.Id))
        {
            Log.Warn($"[PacketManager]: Received packet from unknown peer {e.Peer.Id}. Ignoring.");
            e.Reader.Recycle();
            return;
        }

        ClientManager.TryGetClientById(e.Peer.Id, out var client);

        if (client == null)
        {
            e.Reader.Recycle();
            return;
        }

        var packetType = (PacketType)e.Reader.GetByte();
        var packet = DeserializePacket(packetType, e.Reader);

        if (packet == null)
        {
            Log.Warn($"[PacketManager]: Failed to deserialize packet of {packetType} from {client.Name}");
            e.Reader.Recycle();
            return;
        }

        client.Enqueue(packetType, packet);

        var bucket = GetBucket(packetType);
        Log.Debug($"[PacketManager]: Enqueued {packetType} [{bucket}] from {client.Name}");

        e.Reader.Recycle();
    }

    /// <summary>
    /// Deserializes a packet from the given packet type and reader.
    /// </summary>
    /// <param name="type">The type of the packet to be deserialized.</param>
    /// <param name="reader">The packet data reader used to deserialize the packet.</param>
    /// <returns>The deserialized packet as an instance of <see cref="IRealmPacket"/>, or null if deserialization
    /// fails or the packet type is unsupported.</returns>
    private static IRealmPacket? DeserializePacket(PacketType type, NetPacketReader reader)
    {
        IRealmPacket? packet = type switch
        {
            PacketType.RequestLoginDataPacket => new RequestLoginDataPacket(),
            PacketType.SubmitLoginDataPacket => new SubmitLoginDataPacket(),
            // Character Bucket (10-14)
            // Movement Bucket
            // Combat Bucket
            // General Bucket
            _ => null
        };

        packet?.Deserialize(reader);
        
        return packet;
    }
    #endregion Packet Receiving
}

/*
 *------------------------------------------------------------
 * (PacketManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */