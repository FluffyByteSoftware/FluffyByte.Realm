/*
 * (ClockStoppedEvent.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@4:51:54 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Tools.Broadcasting.Events;

public class ClockStoppedEvent : EventArgs
{
    public string ClockName { get; set; } = string.Empty;
}

/*
 *------------------------------------------------------------
 * (ClockStoppedEvent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */