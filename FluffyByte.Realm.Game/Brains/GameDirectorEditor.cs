/*
 * (GameDirectorEditor.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@11:00:47 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Text.Json;
using System.Xml.Linq;
using FluffyByte.Realm.Shared.Misc;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;

namespace FluffyByte.Realm.Game.Brains;

public class GameDirectorEditor
{
    #region Constants

    public const string ConfigFilePath =
        @"E:\FluffyByte\Builds\0.0.1\ServerData\GameDirectorConfig.json";
    #endregion Constants
    
    #region Load

    public GameDirectorConfig Load()
    {
        var readEvent = new RequestFileReadTextEvent()
        {
            FilePath = ConfigFilePath
        };

        EventManager.Publish(readEvent);

        var text = readEvent.GetText();

        if (string.IsNullOrWhiteSpace(text))
        {
            var defaults = new GameDirectorConfig();
            Save(defaults);

            return defaults;
        }

        try
        {
            var config = JsonSerializer.Deserialize<GameDirectorConfig>(text, FluffyJson.Options);

            if (config is null)
            {
                var defaults = new GameDirectorConfig();
                Save(defaults);

                return defaults;
            }

            return config;
        }
        catch (JsonException)
        {
            var defaults = new GameDirectorConfig();
            Save(defaults);
            
            return defaults;
        }
    }
    #endregion Load
    
    #region Save

    public void Save(GameDirectorConfig config)
    {
        var json = JsonSerializer.Serialize(config, FluffyJson.Options);
        
        EventManager.Publish(new RequestFileWriteTextEvent()
        {
            FilePath = ConfigFilePath,
            Text = json
        });
    }
    #endregion Save
}

/*
 *------------------------------------------------------------
 * (GameDirectorEditor.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */