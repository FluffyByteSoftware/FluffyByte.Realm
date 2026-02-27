/*
 * (NewClientManager.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 14, 2026@8:42:37 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.Events;
using FluffyByte.Realm.Shared.CryptoTool;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Server;

public static class NewClientManager
{
    private const int LoginTimeoutSeconds = 1;

    public static void WelcomeNewClient(RealmClient client)
    {
        ClientManager.AddRealmClient(client);

        var nonce = CryptoManager.GenerateNonce();
        client.AuthNonce = nonce;

        Log.Info($"[NewClientManager]: {client.Name} connected. Sending challenge.");

        var requestPacket = new RequestLoginDataPacket()
        {
            Nonce = nonce
        };

        client.SendPacket(PacketType.RequestLoginDataPacket, requestPacket);

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

        Log.Warn($"[NewClientManager]: {client.Name} timed out. Disconnecting.");
        client.AuthNonce = null;
        client.Disconnect();
    }

    private static void HandleLoginSubmission(RealmClient client, SubmitLoginDataPacket packet)
    {
        Log.Debug($"[NewClientManager]: Received login from {client.Name} with Username: {packet.Username}");

        var account = AccountManager.GetAccountByUsername(packet.Username);

        if (account == null)
        {
            Log.Warn($"[NewClientManager]: Account not found for '{packet.Username}'. "+
                     $"Disconnecting {client.Name}.");

            client.Disconnect();
            return;
        }

        if (client.AuthNonce == null)
            return;
        
        var valid = CryptoManager.ValidateChallengeResponse(
            account.PasswordHash, client.AuthNonce, packet.ChallengeResponse);

        client.AuthNonce = null;
        
        if (!valid)
        {
            Log.Warn($"[NewClientManager]: Authentication failed for '{packet.Username}'. " +
                     $"Disconnecting {client.Name}.");
            
            var failedResponse = new AuthenticationServerResponsePacket()
            {
                CreatedAt = DateTime.UtcNow,
                Success = false
            };

            client.SendPacket(PacketType.AuthenticationServerResponsePacket, failedResponse);
            
            client.Disconnect();
            return;
        }
        
        Log.Debug($"[NewClientManager]: Authentication successful for '{packet.Username}'.");
        
        var respond = new AuthenticationServerResponsePacket()
        {
            CreatedAt = DateTime.UtcNow,
            Success = true
        };

        client.SetAccount(account);

        client.SendPacket(PacketType.AuthenticationServerResponsePacket, respond);

        EventManager.Publish(new OnAuthenticationSuccessEvent()
        {
            AccountName = packet.Username,
            Client = client
        });
    }
}

/*
 *------------------------------------------------------------
 * (NewClientManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */