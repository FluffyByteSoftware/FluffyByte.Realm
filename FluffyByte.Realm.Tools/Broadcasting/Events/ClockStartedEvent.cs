/*
 * (ClockStartedEvent.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@2:50:11 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Tools.Heartbeats;

namespace FluffyByte.Realm.Tools.Broadcasting.Events;

public class ClockStartedEvent : EventArgs
{
    public string ClockName { get; set; } = string.Empty;
}

/*
 *------------------------------------------------------------
 * (ClockStartedEvent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */