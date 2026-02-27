/*
 * (ServerActorMovedPacket.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 3:22:38 PM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using LiteNetLib.Utils;
using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ServerActorMovedPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public long NetworkId { get; set; }
    public int GlobalX { get; set; }
    public int GlobalZ { get; set; }

    public ServerActorMovedPacket() { }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(NetworkId);
        writer.Put(GlobalX);
        writer.Put(GlobalZ);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        NetworkId = reader.GetLong();
        GlobalX = reader.GetInt();
        GlobalZ = reader.GetInt();
    }
}



/*
 *------------------------------------------------------------
 * (ServerActorMovedPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */