/*
 * (IRealmPacket.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 12, 2026@1:33:54 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public interface IRealmPacket : INetSerializable
    {
         DateTime CreatedAt { get; set; }
    }
}

/*
 *------------------------------------------------------------
 * (IRealmPacket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */