/*
 * (GameDirectorConfig.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@11:01:02 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Text.Json.Serialization;

namespace FluffyByte.Realm.Game.Brains;

public class GameDirectorConfig
{
    #region Tick Intervals (milliseconds)

    public int FastIntervalMs { get; set; } = 20;
    public int NormalIntervalMs { get; set; } = 60;
    public int SlowIntervalMs { get; set; } = 100;
    #endregion Tick Intervals
    
    #region Radii

    public int HotRadius { get; set; } = 10;
    public double WarmRadiusMultiplier { get; set; } = 1.4;
    
    [JsonIgnore]
    public int WarmRadius => (int)Math.Round(HotRadius * WarmRadiusMultiplier);

    #endregion Radii
}

/*
 *------------------------------------------------------------
 * (GameDirectorConfig.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */