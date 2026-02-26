using System;
using FluffyByte.Realm.Shared.Misc;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes;

/// <summary>
/// Packet that holds the list of characters the player has available to their account.
/// </summary>
public class CharacterListPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int SlotCount { get; set; }

    public CharacterSlot[] Slots { get; set; }

    public CharacterListPacket()
    {
        SlotCount = 0;
        Slots = [];
    }
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(SlotCount);

        for (var i = 0; i < SlotCount; i++)
        {
            writer.Put(Slots[i].IsEmpty);

            if (Slots[i].IsEmpty)
                continue;

            writer.PutGuid(Slots[i].Id);
            writer.Put(Slots[i].Name);
            writer.Put((byte)Slots[i].ModelType);
            writer.Put((byte)Slots[i].ComplexModelType);
        }
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        SlotCount = reader.GetInt();
        Slots = new CharacterSlot[SlotCount];

        for (var i = 0; i < SlotCount; i++)
        {
            var isEmpty = reader.GetBool();

            if (isEmpty)
            {
                Slots[i] = CharacterSlot.Empty;
                continue;
            }

            Slots[i] = new CharacterSlot
            {
                Id = reader.GetGuid(),
                Name = reader.GetString(),
                ModelType = (PrimitiveModelType)reader.GetByte(),
                ComplexModelType = (ComplexModelType)reader.GetByte()
            };
        }
    }
}
