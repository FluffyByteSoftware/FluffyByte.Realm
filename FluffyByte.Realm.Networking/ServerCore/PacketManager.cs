/*
 * (PacketManager.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 11, 2026@11:03:02 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.LoginServer;
using FluffyByte.Realm.Networking.ServerCore.Clients;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib.Utils;

namespace FluffyByte.Realm.Networking.ServerCore;

public static class PacketManager
{
    public static void Route(RealmClient client, NetDataReader reader)
    {
        if (client.IsDisconnected 
            || reader.AvailableBytes <= 0 
            || client.AuthenticationState == ClientAuthenticationState.Rejected)
            return;

        var packetType = (PacketType)reader.GetByte();

        switch (packetType)
        {
            case PacketType.LoginResponse:
                
                break;
        }
        
        if(client.AuthenticationState is not 
           (ClientAuthenticationState.Rejected 
           or ClientAuthenticationState.Authenticated))
        {
            
        }

    }

    private static bool IsPreAuthPacket(PacketType packetType)
    {
        // Only these packet types are allowed before authentication
        // Define your auth-related packet IDs here
        return packetType is PacketType.PreAuthGeneral or PacketType.LoginRequest;
    }
}

/*
 *------------------------------------------------------------
 * (PacketManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */