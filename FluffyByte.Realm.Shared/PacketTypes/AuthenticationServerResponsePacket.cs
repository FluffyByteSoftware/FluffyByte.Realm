/*
 * (AuthenticationServerResponsePacket.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 12, 2026@10:28:54 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public class AuthenticationServerResponsePacket : IRealmPacket
    {
        public DateTime CreatedAt { get; set; }
        
        public AuthenticationServerResponsePacket()
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
 * (AuthenticationServerResponsePacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */