/*
 * (ExceptionLog.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:56:56 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Text;

namespace FluffyByte.Realm.Tools.Logger;

public class ExceptionLog(Exception ex)
{
    private readonly Exception _exception = ex;
    private const int MaxDepth = 10;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine();
        sb.AppendLine("=== EXCEPTION DETAILS ===");

        var current = _exception;

        var curDepth = 0;

        while (current != null && curDepth < MaxDepth)
        {
            if (curDepth > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"--- Inner Exception (Level: {curDepth}) ---");
            }

            sb.AppendLine($"Exception: {current.Message}");
    
            if(!string.IsNullOrEmpty(current.StackTrace))
                sb.AppendLine($"StackTrace: {current.StackTrace}");

            current = current.InnerException;
            curDepth++;
        }

        if (current != null && curDepth >= MaxDepth)
        {
            sb.AppendLine();
            sb.AppendLine($"... (Additional inner exceptions truncated)");
        }

        sb.AppendLine($"=== END EXCEPTION DETAILS ===");

        return sb.ToString();
    }
}

/*
 *------------------------------------------------------------
 * (ExceptionLog.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */