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

/// <summary>
/// Represents the severity levels of log messages.
/// </summary>
internal enum LogLevel
{
    /// <summary>
    /// A debug message is generally preferred for all messaging that isn't relevant to shipping a production message.
    /// </summary>
    Debug,
    /// <summary>
    /// Low-level message that is relevant to the application's operation
    /// </summary>
    Info,
    /// <summary>
    /// Elevated message that is relevant to the application's operation
    /// </summary>
    Warning,
    /// <summary>
    /// Exceptional message that is relevant to the application's operation
    /// </summary>
    Error
}

/// <summary>
/// Provides static methods for logging messages and exceptions with varying levels of severity.
/// Supports caller information for enhanced debugging capabilities.
/// </summary>
public static class Log
{
    /// <summary>
    /// Logs a debug-level message to the logging system.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by
    /// the compiler.</param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the
    /// compiler.</param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Debug(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Debug, message, null, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs a debug-level message along with exception details to the logging system.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception associated with the log entry.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Debug(string message,
        Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Debug, message, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs a debug-level message to the logging system.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Debug(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Debug, string.Empty, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs an informational-level message to the logging system.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.</param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided by
    /// the compiler.</param>
    public static void Info(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Info, message, null, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs an informational message to the logging system.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception message to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Info(string message, Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Info, message, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs an informational-level message to the logging system.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Info(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Info, string.Empty, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs a warning-level message to the logging system.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Warn(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Warning, message, null, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs a warning-level message to the logging system.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception associated with the warning.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Warn(string message, Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Warning, message, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs a warning-level message to the logging system.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Warn(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Warning, string.Empty, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs an error-level message to the logging system.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Error(string message,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Error, message, null, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs an error-level message to the logging system along with an optional exception.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="ex">The exception associated with the error, if any.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Error(string message, Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Error, message, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Logs an error-level message to the logging system.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="filePath">The source file path of the caller. This is automatically provided by the compiler.
    /// </param>
    /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.
    /// </param>
    /// <param name="lineNumber">The line number in the source file of the caller. This is automatically provided
    /// by the compiler.</param>
    public static void Error(Exception ex,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        WriteLog(LogLevel.Error, string.Empty, ex, filePath, memberName, lineNumber);
    }

    /// <summary>
    /// Writes a log entry with the specified log level, message, exception, and source location details.
    /// </summary>
    /// <param name="level">The severity level of the log entry.</param>
    /// <param name="message">The message to be logged. Can be an empty string if no message is specified.</param>
    /// <param name="ex">The exception to log, or null if no exception is provided.</param>
    /// <param name="filePath">The source file path of the caller. Automatically provided by the compiler.</param>
    /// <param name="memberName">The name of the member where the log entry was triggered. Automatically provided by
    /// the compiler.</param>
    /// <param name="lineNumber">The line number in the source file where the log entry was triggered. Automatically
    /// provided by the compiler.</param>
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