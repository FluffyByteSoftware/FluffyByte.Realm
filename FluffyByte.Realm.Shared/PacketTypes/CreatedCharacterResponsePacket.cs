/*
 * (CreatedCharacterResponsePacket.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@11:29:29 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using FluffyByte.Realm.Shared.Misc;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes;


/// <summary>
/// Packet used to handle responses from the server to client requests to create a new character.
/// </summary>
public class CreateCharacterResponsePacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
    public NameValidationResult Reason { get; set; }

    public CreateCharacterResponsePacket()
    {
        Success = false;
        Reason = NameValidationResult.Empty;
    }
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(Success);
        writer.Put((byte)Reason);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        Success = reader.GetBool();
        Reason = (NameValidationResult)reader.GetByte();
    }
}
/*
 *------------------------------------------------------------
 * (CreatedCharacterResponsePacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */