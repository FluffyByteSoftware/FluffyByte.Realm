/*
 * (DisconnectPacket.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@8:47:40 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using LiteNetLib;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public class DisconnectPacket : INetSerializable
    {
        public DisconnectReason DisconnectReason;
        
        public DisconnectPacket()
        {
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)DisconnectReason);
        }

        public void Deserialize(NetDataReader reader)
        {
            DisconnectReason = (DisconnectReason)reader.GetByte();
        }
        
    }
}

/*
 *------------------------------------------------------------
 * (DisconnectPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */