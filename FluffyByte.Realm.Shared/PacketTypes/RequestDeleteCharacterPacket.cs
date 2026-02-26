/*
 * (RequestDeleteCharacterPacket.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@11:28:54 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using FluffyByte.Realm.Shared.Misc;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes;


/// <summary>
/// Packet used to handle client requests to delete a character.
/// </summary>
public class RequestDeleteCharacterPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CharacterId { get; set; }

    public RequestDeleteCharacterPacket()
    {
        CharacterId = Guid.Empty;
    }
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.PutGuid(CharacterId);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        CharacterId = reader.GetGuid();
    }
}
/*
 *------------------------------------------------------------
 * (RequestDeleteCharacterPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */