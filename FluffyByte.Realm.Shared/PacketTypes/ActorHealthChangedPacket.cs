/*
 * (ServerHealthChanged.cs)
 *------------------------------------------------------------
 * Created - 2/27/2026 3:26:18 PM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using LiteNetLib.Utils;
using System;

namespace FluffyByte.Realm.Shared.PacketTypes;

public class ActorHealthChangedPacket : IRealmPacket
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public long NetworkId { get; set; }
    public int Current { get; set; }
    public int Max { get; set; }
    
    public ActorHealthChangedPacket()
    {
        
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(CreatedAt.Ticks);
        writer.Put(NetworkId);
        writer.Put(Current);
        writer.Put(Max);
    }

    public void Deserialize(NetDataReader reader)
    {
        CreatedAt = new DateTime(reader.GetLong());
        NetworkId = reader.GetLong();
        Current = reader.GetInt();
        Max = reader.GetInt();
    }
}



/*
 *------------------------------------------------------------
 * (ServerHealthChanged.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */