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
        // Authentication Bucket Packets
        // Pre-Authentication (before login)
        RequestLoginDataPacket = 0,
        // Authentication
        SubmitLoginDataPacket = 1,
        AuthenticationServerResponsePacket = 2,
        
        // Character Login Management
        CharacterList = 10,
        CharacterCreate = 11,
        CharacterSelect = 12,
        CharacterDelete = 13,
        CharacterReturn = 14,
        
        // Reserved up to 49
        // Movement Bucket Packets
        ClientRequestZoneLoad = 50,
        UpdateMovementAndRotationClientToServer = 51,
        UpdateMovementAndRotationServerToClient = 52,
        UpdateJumpVectorServerToClient = 53,
        UpdateJumpVectorClientToServer = 54,
        ServerSendZoneToClient = 55,
        
        // Reserved up to 69
        
        // Combat Bucket Packets
        ClientAutoAttack = 70,
        ClientUseAbilityOrSpell = 71,
        ServerToClientStatusUpdate = 72,
        ClientToServerStatusUpdate = 73,
        
        // Reserved up to 99
        
        // General Bucket Packets
        ClientChatMessage = 100,
        ServerChatMessage = 101,
        ClientCommand = 102,
        TimeUpdate = 103
    }
}

/*
 *------------------------------------------------------------
 * (PacketType.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */