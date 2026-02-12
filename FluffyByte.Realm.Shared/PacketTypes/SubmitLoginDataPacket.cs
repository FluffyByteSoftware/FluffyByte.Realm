/*
 * (AuthenticationPackets.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@10:00:34 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public class SubmitLoginDataPacket : INetSerializable
    {
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        public SubmitLoginDataPacket()
        {
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Username);
            writer.PutBytesWithLength(PasswordHash);
        }

        public void Deserialize(NetDataReader reader)
        {
            Username = reader.GetString();
            PasswordHash = reader.GetBytesWithLength();
        }
    }
}

/*
 *------------------------------------------------------------
 * (AuthenticationPackets.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */