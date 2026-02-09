/*
 * (TickEvent.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@3:08:06 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Tools.Broadcasting.Events;

public class TickEvent : EventArgs
{
    public string ClockName { get; set; } = string.Empty;
    public long TickNumber { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public float DeltaTime { get; set; }
}

/*
 *------------------------------------------------------------
 * (TickEvent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */