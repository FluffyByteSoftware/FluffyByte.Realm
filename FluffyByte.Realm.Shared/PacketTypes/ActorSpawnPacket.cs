/*
 * (ActorSpawnPacket.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@4:47:03 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ActorSpawnPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public uint ActorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GlobalX { get; set; }
    public int GlobalZ { get; set; }
    public bool IsUnique { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    public void Serialize(LiteNetLib.Utils.NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(ActorId);
        writer.Put(Name);
        writer.Put(GlobalX);
        writer.Put(GlobalZ);
        writer.Put(IsUnique);
        writer.Put(CurrentHealth);
        writer.Put(MaxHealth);
    }

    public void Deserialize(LiteNetLib.Utils.NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        ActorId = reader.GetUInt();
        Name = reader.GetString();
        GlobalX = reader.GetInt();
        GlobalZ = reader.GetInt();
        IsUnique = reader.GetBool();
        CurrentHealth = reader.GetInt();
        MaxHealth = reader.GetInt();
    }
}

/*
 *------------------------------------------------------------
 * (ActorSpawnPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */