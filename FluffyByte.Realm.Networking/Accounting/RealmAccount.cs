/*
 * (RealmAccount.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 15, 2026@10:18:04 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */
using System.Text;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Networking.Accounting;

public class RealmAccount
{
    private const int MaxCharacterSlots = 3;
    
    public string Username { get; private set; }
    public string FilePath { get; private set; }
    public byte[] PasswordHash { get; private set; }
    public Guid[] Characters { get; private set; }

    private const string AccountsFolder = @"E:\FluffyByte\Builds\0.0.1\ServerData\Accounts";
    
    public RealmAccount(string username, byte[] passwordHash)
    {
        Username = username;
        FilePath = Path.Combine(AccountsFolder, $"{username}.account");
        PasswordHash = passwordHash;
        Characters = new Guid[MaxCharacterSlots];
    }

    private RealmAccount()
    {
        Username = string.Empty;
        FilePath = string.Empty;
        PasswordHash = [];
        Characters = new Guid[MaxCharacterSlots];
    }

    /// <summary>
    /// Load a RealmAccount from a file
    ///
    /// Expected format:
    /// #USERNAME=value
    /// #HASHPASSWORD=hex
    /// #CHARACTER1=guid (or empty)
    /// #CHARACTER2=guid (or empty)
    /// #CHARACTER3=guid (or empty)
    /// </summary>
    /// <param name="filePath">Path to the file to load</param>
    /// <returns>A constructed realm account if found, from the filepath specified.</returns>
    public static RealmAccount? Load(string filePath)
    {
        var read = new RequestFileReadEvent()
        {
            FilePath = filePath
        };
        
        EventManager.Publish(read);
        
        var data = read.GetData();
        
        if (data == null) return null;
        
        try
        {
            var content = Encoding.UTF8.GetString(data);
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries
                                            | StringSplitOptions.TrimEntries);

            var account = new RealmAccount
            {
                FilePath = filePath
            };

            foreach (var line in lines)
            {
                if (!line.StartsWith('#') || !line.Contains('='))
                    continue;

                var key = line[1..line.IndexOf('=')];
                var value = line[(line.IndexOf('=') + 1)..];

                // If you expand beyond character 3, you will need to add additional reserves here.
                switch (key)
                {
                    case "USERNAME":
                        account.Username = value;
                        break;
                    case "HASHPASSWORD":
                        account.PasswordHash = Convert.FromHexString(value);
                        break;
                    case "CHARACTER1":
                        account.Characters[0] = ParseGuid(value);
                        break;
                    case "CHARACTER2":
                        account.Characters[1] = ParseGuid(value);
                        break;
                    case "CHARACTER3":
                        account.Characters[2] = ParseGuid(value);
                        break;
                }
            }

            if (string.IsNullOrEmpty(account.Username) || account.PasswordHash.Length == 0)
            {
                Log.Warn($"[RealmAccount]: Failed to load account from {filePath}. Missing required fields.");
                return null;
            }

            return account;
        }
        catch (Exception ex)
        {
            Log.Error($"Error loading account: {filePath}", ex);
            return null;
        }
    }

    public void Save()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"#USERNAME={Username}");
        sb.AppendLine($"#HASHPASSWORD={Convert.ToHexString(PasswordHash)}");

        for (var i = 0; i < Characters.Length; i++)
        {
            sb.AppendLine($"#CHARACTER{i + 1}={GuidToString(Characters[i])}");
        }

        var write = new RequestFileWriteTextEvent()
        {
            FilePath = Path.Combine(AccountsFolder, $"{Username}.account"),
            Text = sb.ToString()
        };
        
        EventManager.Publish(write);
    }

    public void AddCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
            return;

        if (Characters.Contains(characterId))
            return;

        for (var i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == Guid.Empty)
            {
                Characters[i] = characterId;
                Save();
                return;
            }
        }

        Log.Warn($"[RealmAccount]: Cannot add character {characterId} to {Username}");
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
                return;
            }
        }
        
        Log.Warn($"[RealmAccount]: Cannot remove character {characterId} from {Username}");
    }
    
    private static Guid ParseGuid(string value)
    {
        return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
    }

    private static string GuidToString(Guid guid)
    {
        return guid == Guid.Empty ? "" : guid.ToString();
    }
}

/*
 *------------------------------------------------------------
 * (RealmAccount.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */