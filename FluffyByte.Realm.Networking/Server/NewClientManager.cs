/*
 * (NewClientManager.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@8:42:37 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Server;

public static class NewClientManager
{
    private const int LoginTimeoutSeconds = 1;
    
    public static void WelcomeNewClient(RealmClient client)
    {
        ClientManager.AddRealmClient(client);

        Log.Debug($"[NewClientManager]: {client.Name} connected. Sending login request.");
        
        client.SendPacket(PacketType.RequestLoginDataPacket, new RequestLoginDataPacket());

        _ = WaitForLoginAsync(client);
    }

    private static async Task WaitForLoginAsync(RealmClient client)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(LoginTimeoutSeconds);

        while (DateTime.UtcNow < deadline)
        {
            var packets = client.DrainQueue(PacketType.SubmitLoginDataPacket);

            if (packets.Count > 0)
            {
                var loginPacket = (SubmitLoginDataPacket)packets[0];
                HandleLoginSubmission(client, loginPacket);
                return;
            }

            await Task.Delay(100);
        }

        Log.Warn($"[NewClientManager]: {client.Name} failed to respond within time.  Disconnecting.");
        
        client.Disconnect();
    }

    private static void HandleLoginSubmission(RealmClient client, SubmitLoginDataPacket loginPacket)
    {
        Log.Info($"[NewClientManager]: Received login data from {client.Name}");

        var username = loginPacket.Username;
        var challengeResponse = loginPacket.ChallengeResponse;
        
        
    }
}

/*
 *------------------------------------------------------------
 * (NewClientManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */