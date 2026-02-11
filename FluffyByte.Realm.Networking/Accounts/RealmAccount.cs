/*
 * (RealmAccount.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 10, 2026@6:43:53 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Text;
using System.Text.Json;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Accounts;

public class RealmAccount
{
    public string Username { get; set; } = string.Empty;
    public byte[] EncryptedPassword { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsBanned { get; set; }
    public string[] CharacterNames { get; set; } = [];

    private const string AccountDirectory = @"E:\FluffyByte\Builds\0.0.1\ServerData\Accounts";

    public void Save()
    {
        if (string.IsNullOrEmpty(Username))
            throw new InvalidOperationException("Cannot save an account with no username!");

        if (!Directory.Exists(AccountDirectory))
            Directory.CreateDirectory(AccountDirectory);

        var filePath = GetAccountFilePath(Username);
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions{ WriteIndented = true});

        var data = Encoding.UTF8.GetBytes(json);
        
        EventManager.Publish(new RequestFileWriteEvent()
        {
            FilePath = filePath,
            Data = data
        });
    }

    public static RealmAccount? Load(string username)
    {
        var filePath = GetAccountFilePath(username);
        var readEvent = new RequestFileReadEvent { FilePath = filePath };
        EventManager.Publish(readEvent);

        var data = readEvent.GetData();

        if (data == null || data.Length == 0)
            return null;

        try
        {
            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<RealmAccount>(json);
        }
        catch (Exception ex)
        {
            Log.Error($"Error loading account: {username}", ex);
            return null;
        }
    }

    public static bool Exists(string username)
    {
        if (string.IsNullOrEmpty(username))
            return false;

        var filePath = GetAccountFilePath(username);
        var readEvent = new RequestFileReadEvent { FilePath = filePath };
        EventManager.Publish(readEvent);

        return readEvent.GetData() != null;
    }

    public static RealmAccount Create(string username, byte[] encryptedPassword)
    {
        var account = new RealmAccount()
        {
            Username = username,
            EncryptedPassword = encryptedPassword,
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow,
            IsBanned = false,
            CharacterNames = []
        };

        account.Save();
        
        return account;
    }

    private static string GetAccountFilePath(string username)
    {
        var safeUsername = username.ToLowerInvariant().Replace("..", "").Replace("/", "").Replace("\\", "");

        return Path.Combine(AccountDirectory, $"{safeUsername}.account");
    }
}

/*
 *------------------------------------------------------------
 * (RealmAccount.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */