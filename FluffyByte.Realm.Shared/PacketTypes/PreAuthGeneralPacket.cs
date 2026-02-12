/*
 * (PreAuthGeneralPacket.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 10, 2026@7:27:47 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public class PreAuthGeneralPacket : INetSerializable
    {
        /// <summary>
        /// -1 = Username/Password Rejected
        /// 0 = Request Username/Password
        /// 1 = Username/Password Accepted
        /// </summary>
        public int Header { get; set; } = 0;
        public string Message { get; set; } = string.Empty;

        public PreAuthGeneralPacket()
        {
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Header);
            writer.Put(Message);
        }
        
        public void Deserialize(NetDataReader reader)
        {
            Header = reader.GetInt();
            Message = reader.GetString();
        }
    }
}

/*
 *------------------------------------------------------------
 * (PreAuthGeneralPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */