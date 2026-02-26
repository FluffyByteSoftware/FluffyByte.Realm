/*
 * (NetSerializerExtensions.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@8:35:37 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.Misc;

public static class NetSerializerExtensions
{
    public static void PutGuid(this NetDataWriter writer, Guid guid)
    {
        var bytes = guid.ToByteArray();
        for (var i = 0; i < 16; i++)
        {
            writer.Put(bytes[i]);
        }
    }

    public static Guid GetGuid(this NetPacketReader reader)
    {
        var bytes = new byte[16];
        
        for (var i = 0; i < 16; i++)
        {
            bytes[i] = reader.GetByte();
        }

        return new Guid(bytes);
    }
}

/*
 *------------------------------------------------------------
 * (NetSerializerExtensions.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */