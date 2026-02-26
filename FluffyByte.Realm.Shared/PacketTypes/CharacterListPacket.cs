/*
 * (CharacterListPacket.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@2:22:32 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using System.Security.Principal;
using FluffyByte.Realm.Shared.Misc;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class CharacterListPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int SlotCount { get; set; }
    public CharacterSlot[] Slots { get; set; } = [];

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(SlotCount);

        for (var i = 0; i < SlotCount; i++)
        {
            writer.Put(Slots[i].IsEmpty);

            if (Slots[i].IsEmpty)
                continue;
            
            writer.Put(Slots[i].Id.ToString());
            writer.Put(Slots[i].Name);
        }
    }

    public void Deserialize(NetDataReader reader)
    {
        SlotCount = reader.GetInt();

        Slots = new CharacterSlot[SlotCount];

        for (var i = 0; i < SlotCount; i++)
        {
            var isEmpty = reader.GetBool();

            Slots[i] = isEmpty
                ? CharacterSlot.Empty
                : new CharacterSlot
                {
                    Id = Guid.Parse(reader.GetString()),
                    Name = reader.GetString()
                };
        }
    }
}

/*
 *------------------------------------------------------------
 * (CharacterListPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */