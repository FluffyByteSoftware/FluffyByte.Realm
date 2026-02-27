/*
 * (ServerDespawnActorPacket.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 12:44:07 PM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using LiteNetLib.Utils;
using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ServerDespawnActorPacket : IRealmPacket
{
    public ServerDespawnActorPacket() { }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long NetworkId { get; set; }

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
* (ServerDespawnActorPacket.cs)
* See License.txt for licensing information.
*-----------------------------------------------------------
*/