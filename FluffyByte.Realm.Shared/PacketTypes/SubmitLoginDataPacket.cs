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
    public class SubmitLoginDataPacket : IRealmPacket
    {
        public string Username { get; set; }
        public byte[] ChallengeResponse { get; set; }
        public DateTime CreatedAt { get; set; }

        public SubmitLoginDataPacket()
        {
            CreatedAt = DateTime.UtcNow;
            Username = string.Empty;
            ChallengeResponse = Array.Empty<byte>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(CreatedAt.Ticks);
            writer.Put(Username);
            writer.PutBytesWithLength(ChallengeResponse);
        }

        public void Deserialize(NetDataReader reader)
        {
            CreatedAt = new DateTime(reader.GetLong());
            Username = reader.GetString();
            ChallengeResponse = reader.GetBytesWithLength();
        }
    }
}

/*
 *------------------------------------------------------------
 * (AuthenticationPackets.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */