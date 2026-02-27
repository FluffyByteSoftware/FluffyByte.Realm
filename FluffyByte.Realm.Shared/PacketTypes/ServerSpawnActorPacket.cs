/*
 * (ServerSpawnActorPacket.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 12:37:57 PM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using FluffyByte.Realm.Shared.Misc;
using LiteNetLib.Utils;
using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ServerSpawnActorPacket : IRealmPacket
{
    public ServerSpawnActorPacket()
    {

    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long NetworkId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GlobalX { get; set; }
    public int GlobalZ { get; set; }
    public float Rotation { get; set; }
    public PrimitiveModelType ModelType { get; set; }
    public ComplexModelType ComplexModelType { get; set; }
    public bool HasComplexModel { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public bool IsSelf { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(NetworkId);
        writer.Put(Name);
        writer.Put(GlobalX);
        writer.Put(GlobalZ);
        writer.Put(Rotation);
        writer.Put((byte)ModelType);
        writer.Put(HasComplexModel);
        if (HasComplexModel)
            writer.Put((byte)ComplexModelType);
        writer.Put(CurrentHealth);
        writer.Put(MaxHealth);
        writer.Put(IsSelf);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        NetworkId = reader.GetLong();
        Name = reader.GetString();
        GlobalX = reader.GetInt();
        GlobalZ = reader.GetInt();
        Rotation = reader.GetFloat();
        ModelType = (PrimitiveModelType)reader.GetByte();
        HasComplexModel = reader.GetBool();
        if (HasComplexModel)
            ComplexModelType = (ComplexModelType)reader.GetByte();
        CurrentHealth = reader.GetInt();
        MaxHealth = reader.GetInt();
        IsSelf = reader.GetBool();
    }
}

/*
*------------------------------------------------------------
* (ServerSpawnActorPacket.cs)
* See License.txt for licensing information.
*-----------------------------------------------------------
*/