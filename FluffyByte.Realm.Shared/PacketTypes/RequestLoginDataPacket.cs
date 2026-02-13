/*
 * (RequestLoginDataPacket.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 12, 2026@9:53:20 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public class RequestLoginDataPacket : IRealmPacket
    {
        public DateTime CreatedAt { get; set; }
        
        public RequestLoginDataPacket()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(CreatedAt.Ticks);
        }

        public void Deserialize(NetDataReader reader)
        {
            CreatedAt = new DateTime(reader.GetLong());
        }
    }
}

/*
 *------------------------------------------------------------
 * (RequestLoginDataPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */