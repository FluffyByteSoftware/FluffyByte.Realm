/*
 * (LogWriteEvent.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@12:38:21 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Tools.Broadcasting.Events;

public class LogEvents(string message) : EventArgs
{
    public string Message { get; set; } = message;
}

/*
 *------------------------------------------------------------
 * (LogWriteEvent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */