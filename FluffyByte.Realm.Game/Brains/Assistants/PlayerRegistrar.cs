/*
 * (PlayerRegistrar.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@10:59:36 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Text.Json;
using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Shared.Misc;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains.Assistants;

public class PlayerRegistrar
{
    private readonly Dictionary<Guid, PlayerProfile> _profiles = [];
    private readonly string _savePath;

    public PlayerRegistrar()
    {
        _savePath = GameDirector.Config.CharacterDataPath;
        
        Directory.CreateDirectory(_savePath);
    }
    
    #region Create/Delete
    public void Register(PlayerProfile profile)
    {
        _profiles[profile.Id] = profile;
        Save(profile);
    }

    public void DeleteProfile(Guid id)
    {
        if (!_profiles.TryGetValue(id, out var profile))
        {
            return;
        }

        var path = GetFilePath(profile.Name);
        if (File.Exists(path))
            File.Delete(path);

        _profiles.Remove(id);
    }
    #endregion Create/Delete
    
    #region Load/Save

    public PlayerProfile? LoadProfile(string name)
    {
        var path = GetFilePath(name);
        
        if (!File.Exists(path))
            return null;

        var requestRead = new RequestFileReadTextEvent()
        {
            FilePath = path
        };
        
        EventManager.Publish(requestRead);

        var json = requestRead.GetText();

        if (json == null)
        {
            Log.Warn($"[PlayerRegistrar]: Failed to read {path}");
            
            return null;
        }
        
        var profile = JsonSerializer.Deserialize<PlayerProfile>(json, FluffyJson.Options);

        if (profile == null)
            return null;

        _profiles[profile.Id] = profile;
        
        return profile;
    }

    public void Save(PlayerProfile profile)
    {
        var path = GetFilePath(profile.Name);
        
        string json = JsonSerializer.Serialize(profile, FluffyJson.Options);

        var requestWrite = new RequestFileWriteTextEvent()
        {
            FilePath = path,
            Text = json
        };

        EventManager.Publish(requestWrite);
    }

    public void SaveAll()
    {
        foreach (var profile in _profiles.Values)
            Save(profile);
    }
    #endregion Load/Save
    
    #region Query

    public PlayerProfile? GetById(Guid id)
    {
        _profiles.TryGetValue(id, out var profile);
        return profile;
    }

    public PlayerProfile? GetByName(string name)
    {
        return _profiles.Values.FirstOrDefault(p => p.Name.Equals(name,
            StringComparison.OrdinalIgnoreCase));
    }
    
    public bool ProfileExists(string name) => File.Exists(GetFilePath(name));
    #endregion Query
    
    
    #region Helpers

    public string GetFilePath(string name) => Path.Combine(_savePath, $"{name}.RealmCharacter");
    #endregion helpers
    
    #region Diagnostics
    public int LoadedCount => _profiles.Count;
    
    public override string ToString() => $"PlayerRegistrar Loaded={LoadedCount}";
    #endregion Diagnostics
}

/*
 *------------------------------------------------------------
 * (PlayerRegistrar.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */