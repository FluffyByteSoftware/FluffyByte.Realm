/*
 * (RealmDispatcher.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@7:40:51 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.Server;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Dispatcher;

public static class RealmDispatcher
{
    private static readonly Dictionary<PacketBucket, PacketType[]> _bucketToTypes = [];

    static RealmDispatcher()
    {
        foreach (PacketBucket bucket in Enum.GetValues<PacketBucket>())
        {
            var typesInBucket = new List<PacketType>();
            
            foreach (PacketType type in Enum.GetValues<PacketType>())
            {
                if (PacketManager.GetBucket(type) == bucket)
                {
                    typesInBucket.Add(type);
                }
            }

            _bucketToTypes[bucket] = [..typesInBucket];
        }
    }

    public static void Pulse(PacketBucket bucket)
    {
        if (!_bucketToTypes.TryGetValue(bucket, out var validTypes))
        {
            return;
        }

        foreach (var client in ClientManager.Clients.Values)
        {
            if (!client.IsConnected) continue;

            foreach (var type in validTypes)
            {
                var packets = client.DrainQueue(type);

                foreach (var packet in packets)
                {
                    Execute(client, type, packet);
                }
            }
        }
    }

    private static void Execute(RealmClient client, PacketType type, IRealmPacket packet)
    {
        try
        {
            switch (type)
            {
                case PacketType.SubmitLoginDataPacket:
                    break;
                case PacketType.UpdateMovementAndRotationClientToServer:
                    break;
                default:
                    Log.Warn($"[Dispatcher]: Unahndled packet type: {type} from {client.Name}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[Dispatcher]: Critical error executing {type} for {client.Name}: {ex.Message}");
        }
    }

    private static void HandleMovement(RealmClient client, IRealmPacket packet)
    {
        // Logic: Extract target location from packet
        // Queue command to the Tile Command Queue or FastTick bucket on the Actor
        // TODO: Implement movement handling logic
    }
}

/*
 *------------------------------------------------------------
 * (RealmDispatcher.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */