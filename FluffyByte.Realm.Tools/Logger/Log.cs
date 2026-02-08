/*
 * (Log.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:51:27 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Runtime.CompilerServices;
using System.Text;

namespace FluffyByte.Realm.Tools.Logger;

internal enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

public static class Log
{
    public static void Debug(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Debug, message, null, filePath, memberName, lineNumber);
    }

    public static void Debug(string message,
        Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Debug, message, ex, filePath, memberName, lineNumber);
    }

    public static void Debug(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Debug, string.Empty, ex, filePath, memberName, lineNumber);
    }

    public static void Info(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Info, message, null, filePath, memberName, lineNumber);
    }

    public static void Info(string message, Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Info, message, ex, filePath, memberName, lineNumber);
    }

    public static void Info(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Info, string.Empty, ex, filePath, memberName, lineNumber);
    }
    
    public static void Warn(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Warning, message, null, filePath, memberName, lineNumber);
    }

    public static void Warn(string message, Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Warning, message, ex, filePath, memberName, lineNumber);
    }

    public static void Warn(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Warning, string.Empty, ex, filePath, memberName, lineNumber);
    }
    
    public static void Error(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Error, message, null, filePath, memberName, lineNumber);
    }

    public static void Error(string message, Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Error, message, ex, filePath, memberName, lineNumber);
    }

    public static void Error(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Error, string.Empty, ex, filePath, memberName, lineNumber);
    }
    
    private static void WriteLog(
        LogLevel level, 
        string message, 
        Exception? ex, 
        string filePath, 
        string memberName,
        int lineNumber)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd@HH:mm:ss");
        var fileName = Path.GetFileName(filePath);
        var location = $"{fileName} @ {memberName}: {lineNumber}";

        var logBuilder = new StringBuilder();

        logBuilder.Append($"[{timestamp}] - [{level}] - ");

        if (!string.IsNullOrEmpty(message))
            logBuilder.Append(message);

        if (ex != null)
        {
            if (!string.IsNullOrEmpty(message))
                logBuilder.Append(" - ");

            var exLog = new ExceptionLog(ex);
            logBuilder.Append(exLog);
        }

        logBuilder.Append($" - [{location}]");

        Console.WriteLine(logBuilder.ToString());
    }
}

/*
 *------------------------------------------------------------
 * (Log.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */