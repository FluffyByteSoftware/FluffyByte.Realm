using System;
using FluffyByte.Realm.Shared.Misc;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes;


/// <summary>
/// Selects a character from the list of characters the player has available.
/// </summary>
public class SelectCharacterPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CharacterGuid { get; set; }

    public SelectCharacterPacket()
    {
        CharacterGuid = Guid.Empty;
    }
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.PutGuid(CharacterGuid);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        CharacterGuid = reader.GetGuid();
    }
}
