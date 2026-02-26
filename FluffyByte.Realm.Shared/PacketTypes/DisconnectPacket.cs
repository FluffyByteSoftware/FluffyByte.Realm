/*
 * (DisconnectPacket.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@8:47:40 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public class DisconnectPacket : IRealmPacket
    {
        public DateTime CreatedAt { get; set; }
        public DisconnectReason DisconnectReason;
        
        public DisconnectPacket()
        {
            CreatedAt = DateTime.UtcNow;
            DisconnectReason = DisconnectReason.RemoteConnectionClose;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(CreatedAt.Ticks);
            writer.Put((byte)DisconnectReason);
        }

        public void Deserialize(NetDataReader reader)
        {
            DisconnectReason = (DisconnectReason)reader.GetByte();
            CreatedAt = new DateTime(reader.GetLong());
        }
        
    }
}

/*
 *------------------------------------------------------------
 * (DisconnectPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */