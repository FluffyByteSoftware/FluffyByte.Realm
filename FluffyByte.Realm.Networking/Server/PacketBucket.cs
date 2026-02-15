/*
 * (PacketBucket.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 15, 2026@12:07:07 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Networking.Server;

public enum PacketBucket : byte
{
    Auth,       // 0-49
    Movement,   // 50-69
    Combat,     // 70-99
    General,    // 100+
}

/*
 *------------------------------------------------------------
 * (PacketBucket.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */