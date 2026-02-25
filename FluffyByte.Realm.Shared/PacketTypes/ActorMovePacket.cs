/*
 * (ActorMovePacket.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@6:43:19 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ActorMovePacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public uint ActorId { get; set; }
    public int NewX { get; set; }
    public int NewZ { get; set; }

    public void Serialize(LiteNetLib.Utils.NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(ActorId);
        writer.Put(NewX);
        writer.Put(NewZ);
    }

    public void Deserialize(LiteNetLib.Utils.NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        ActorId = reader.GetUInt();
        NewX = reader.GetInt();
        NewZ = reader.GetInt();
    }
}

/*
 *------------------------------------------------------------
 * (ActorMovePacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */