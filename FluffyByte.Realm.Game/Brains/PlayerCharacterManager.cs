/*
 * (PlayerCharacterManager.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@3:48:42 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Brains.Helpers;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains;

public static class PlayerCharacterManager
{
    private static bool _isInitialized;

    private const int MinNameLength = 3;
    private const int MaxNameLength = 18;

    #region Lifecycle
    public static void Initialization()
    {
        if (_isInitialized)
            return;

        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
        
        _isInitialized = true;
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized) return;
        
        _isInitialized = false;
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
    }
    #endregion Lifecycle

    public static PlayerProfile? CreateCharacter(string name, RealmAccount account)
    {
        var validation = ValidateName(name);

        if (validation != NameValidationResult.Valid)
        {
            Log.Warn($"[PlayerCharacterManager]: Character name '{name}' rejected: {validation}");
            return null;
        }

        if (!HasEmptySlot(account))
        {
            Log.Warn($"[PlayerCharacterManager]: Account '{account.Username}' has no empty character slots.");
            return null;
        }

        GameDirector.CreatePlayerProfile(name);

        var profile = GameDirector.GetPlayerProfile(name);

        if (profile == null)
        {
            Log.Warn($"[PlayerCharacterManager]: Failed to create character '{name}' for account " +
                     $"'{account.Username}'.");

            return null;
        }

        account.AddCharacter(profile.Id);

        return profile;
    }

    public static bool DeleteCharacter(Guid characterId, RealmAccount account)
    {
        if (characterId == Guid.Empty)
            return false;

        if (!account.Characters.Contains(characterId))
        {
            Log.Warn($"[PlayerCharacterManager]: Character {characterId} not found on account '{account.Username}'.");

            return false;
        }

        account.RemoveCharacter(characterId);
        GameDirector.DeletePlayerProfile(characterId);

        Log.Info($"[PlayerCharacterManager]: Deleted character {characterId} from account '{account.Username}'.");
        
        return true;
    }

    public static NameValidationResult ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return NameValidationResult.Empty;

        if (name.Length < MinNameLength)
            return NameValidationResult.TooShort;

        if (name.Length < MaxNameLength)
            return NameValidationResult.TooLong;

        if (!name.All(char.IsLetter))
            return NameValidationResult.InvalidCharacters;

        if (GameDirector.PlayerProfileExists(name))
            return NameValidationResult.AlreadyTaken;

        return NameValidationResult.Valid;
    }

    private static bool HasEmptySlot(RealmAccount account)
    {
        return account.Characters.Any(g => g == Guid.Empty);
    }
    
}

/*
 *------------------------------------------------------------
 * (PlayerCharacterManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */