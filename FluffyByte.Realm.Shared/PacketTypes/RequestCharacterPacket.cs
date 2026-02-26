/*
 * (RequestCharacterPacket.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@11:28:18 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes;


/// <summary>
/// Packet used to handle client requests to create a new character.
/// </summary>
public class RequestCreateCharacterPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }

    public RequestCreateCharacterPacket()
    {
        CreatedAt = DateTime.UtcNow;
        Name = string.Empty;
    }
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(Name);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        Name = reader.GetString();
    }
}


/*
 *------------------------------------------------------------
 * (RequestCharacterPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */