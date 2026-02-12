/*
 * (PacketType.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 10, 2026@1:22:04 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Shared.PacketTypes
{
    public enum PacketType : byte
    {
        // Pre-Authentication (before login)
        RequestLoginDataPacket = 0,
        // Authentication
        SubmitLoginDataPacket = 1,
        AuthenticationServerResponsePacket = 2,
        
        // Character Management
        CharacterList = 10,
        CharacterCreate = 11,
        CharacterSelect = 12,
        
        // Game Packet Handling
        EnterWorld = 100,
        Movement = 101,
        ChatMessage = 102
    }
}

/*
 *------------------------------------------------------------
 * (PacketType.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */