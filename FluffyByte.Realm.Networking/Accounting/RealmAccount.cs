/*
 * (RealmAccount.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 15, 2026@10:18:04 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */
using System.Text.Json;
using System.Text.Json.Serialization;
using FluffyByte.Realm.Shared.Misc;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Accounting;
public class RealmAccount
{
    public const int MaxCharacterSlots = 4;

    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public Guid[] Characters { get; set; } = new Guid[MaxCharacterSlots];

    [JsonIgnore]
    public string FilePath { get; set; } = string.Empty;

    private const string AccountsFolder = @"E:\FluffyByte\Builds\0.0.1\ServerData\Accounts";

    public RealmAccount() { }

    public RealmAccount(string username, byte[] passwordHash)
    {
        if (username.Length is < 3 or > 20)
        {
            throw new ArgumentException("Username must be between 3 and 20 characters.");
        }

        if (!username.All(char.IsLetter))
        {
            throw new ArgumentException("Username must contain only letters.");
        }
        
        Username = username;
        PasswordHash = passwordHash;
        FilePath = Path.Combine(AccountsFolder, $"{username}.account");
    }

    public static RealmAccount? Load(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        var read = new RequestFileReadTextEvent { FilePath = filePath };
        EventManager.Publish(read);

        var json = read.GetText();
        if (json == null)
        {
            Log.Warn($"[RealmAccount]: Failed to read {filePath}");
            return null;
        }

        try
        {
            var account = JsonSerializer.Deserialize<RealmAccount>(json, FluffyJson.Options);

            Console.WriteLine($"Account deserialized.\n " +
                $"Username: {account?.Username}");

            if (account == null) return null;

            account.FilePath = filePath;

            if (string.IsNullOrEmpty(account.Username) || account.PasswordHash.Length == 0)
            {
                Log.Warn($"[RealmAccount]: Missing required fields in {filePath}; missing account.Username or account.PasswordHash");
                Log.Warn($"[RealmAccount]: account.Username: '{account.Username}', account.PasswordHash length: {account.PasswordHash.Length}");
                return null;
            }

            // Handle old 3-slot files upgrading to 4
            if (account.Characters.Length < MaxCharacterSlots)
            {
                var expanded = new Guid[MaxCharacterSlots];
                Array.Copy(account.Characters, expanded, account.Characters.Length);
                account.Characters = expanded;
            }

            return account;
        }
        catch (Exception ex)
        {
            Log.Error($"[RealmAccount]: Error loading {filePath}", ex);
            return null;
        }
    }

    public void Save()
    {
        var path = string.IsNullOrEmpty(FilePath)
            ? Path.Combine(AccountsFolder, $"{Username}.account")
            : FilePath;

        var json = JsonSerializer.Serialize(this, FluffyJson.Options);

        var write = new RequestFileWriteTextEvent
        {
            FilePath = path,
            Text = json
        };

        EventManager.Publish(write);
    }

    public bool AddCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
            return false;

        if (Characters.Contains(characterId))
            return false;

        for (var i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == Guid.Empty)
            {
                Characters[i] = characterId;
                Save();
                return true;
            }
        }

        Log.Warn($"[RealmAccount]: No empty slots for {Username}");
        return false;
    }

    public void RemoveCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
            return;

        for (var i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == characterId)
            {
                Characters[i] = Guid.Empty;
                Save();
                return;
            }
        }
    }
}

/*
 *------------------------------------------------------------
 * (RealmAccount.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */