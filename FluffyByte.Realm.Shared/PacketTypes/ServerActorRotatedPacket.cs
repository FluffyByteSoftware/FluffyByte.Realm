/*
 * (ServerActorRotatedPacket.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 3:24:49 PM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using LiteNetLib.Utils;
using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ServerActorRotatedPacket : IRealmPacket
{
    public ServerActorRotatedPacket() { }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long NetworkId { get; set; }
    public float Rotation { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(NetworkId);
        writer.Put(Rotation);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        NetworkId = reader.GetLong();
        Rotation = reader.GetFloat();
    }
}



/*
 *------------------------------------------------------------
 * (ServerActorRotatedPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */