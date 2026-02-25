/*
 * (ActorDespawnPacket.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@7:35:09 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ActorDespawnPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public uint ActorId { get; set; }

    public void Serialize(LiteNetLib.Utils.NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(ActorId);
    }

    public void Deserialize(LiteNetLib.Utils.NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        ActorId = reader.GetUInt();
    }
}

/*
 *------------------------------------------------------------
 * (ActorDespawnPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */