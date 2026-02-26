/*
 * (LoginHandlerPackets.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@10:24:34 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes;


/// <summary>
/// Packet used to handle responses from the server to client requests to delete a character.
/// </summary>
public class DeleteCharacterResponsePacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; }
    public bool Success { get; set; }

    public DeleteCharacterResponsePacket()
    {
        CreatedAt = DateTime.UtcNow;
        Success = false;
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(Success);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        Success = reader.GetBool();
    }
}

/// <summary>
/// Packet used to handle responses from the server to client requests to select a character.
/// </summary>
public class CharacterSelectedPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
    public long NetworkId { get; set; }
    public int GlobalX { get; set; }
    public int GlobalZ { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(Success);
        writer.Put(NetworkId);
        writer.Put(GlobalX);
        writer.Put(GlobalZ);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        Success = reader.GetBool();
        NetworkId = reader.GetLong();
        GlobalX = reader.GetInt();
        GlobalZ = reader.GetInt();
    }
}
/*
 *------------------------------------------------------------
 * (LoginHandlerPackets.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */