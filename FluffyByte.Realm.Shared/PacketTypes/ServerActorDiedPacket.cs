/*
 * (ServerActorDiedPacket.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 12:47:48 PM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using LiteNetLib.Utils;
using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ServerActorDiedPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long NetworkId { get; set; }

    public ServerActorDiedPacket() { }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(NetworkId);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        NetworkId = reader.GetLong();
    }
}



/*
 *------------------------------------------------------------
 * (ServerActorDiedPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */