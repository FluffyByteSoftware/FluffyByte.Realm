/*
 * (DiskManager.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:58:53 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using System.Text;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Heartbeats;

namespace FluffyByte.Realm.Tools.Disk;

public static class DiskManager
{
    private static readonly ConcurrentDictionary<string, CachedFile> FileCache = [];
    private static readonly ConcurrentQueue<LogEntry> LogBuffer = [];

    private static readonly Lock FileLock = new Lock();
    private static readonly Lock LogLock = new Lock();

    private const int MaxCacheSizeBytes = 50 * 1024 * 1024; // 50 MB
    private const int TicksBetweenFlushes = 900; // 15 minutes at 10 ticks/sec (100 ms tick)
    private const int MaxLogBufferEntries = 1000;

    private static Clock? _clock;
    
    private static long _currentCacheSize;
    private static long _lastFlushTick;
    private static bool _initialized;

    private const string LogDirectory = @$"E:\FluffyByte\Builds\0.0.1\ServerData\Logs\";

    public static void Initialize()
    {
        if (_initialized)
            return;

        EventManager.Subscribe<RequestFileWriteByteEvent>(OnRequestFileByteWrite);
        EventManager.Subscribe<RequestFileWriteTextEvent>(OnRequestFileTextWrite);
        EventManager.Subscribe<RequestFileReadEvent>(OnRequestFileRead);
        EventManager.Subscribe<RequestFileReadTextEvent>(OnRequestFileReadText);
        EventManager.Subscribe<LogEvents>(OnLogWrite);
        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
        
        _initialized = true;
        _clock = ClockManager.RegisterClock("DiskManager", 1000);
        _clock.OnTick += OnTick;
        ClockManager.StartClock(_clock);
        
        Console.WriteLine("[DiskManager]: Initialized.");
    }

    private static void OnTick()
    {
        if (_clock == null || !_initialized)
            return;

        lock (FileLock)
        {
            if (_clock.CurrentTick - _lastFlushTick >= TicksBetweenFlushes)
            {
                Console.WriteLine("[DiskManager]: Periodic flush interval reached. Flushing buffers.");
                Flush();
                _lastFlushTick = _clock.CurrentTick;
            }
        }
    }

    // -------------------------------------------------------------------------
    // Write Handlers
    // -------------------------------------------------------------------------

    private static void OnRequestFileByteWrite(RequestFileWriteByteEvent e)
    {
        try
        {
            lock (FileLock)
            {
                if (FileCache.TryGetValue(e.FilePath, out var cachedFile))
                {
                    var oldSize = cachedFile.Data.Length;
                    cachedFile.Data = e.Data;
                    cachedFile.IsDirty = true;
                    cachedFile.LastAccessed = DateTime.Now;
                    _currentCacheSize = _currentCacheSize - oldSize + e.Data.Length;
                }
                else
                {
                    FileCache[e.FilePath] = new CachedFile
                    {
                        FilePath = e.FilePath,
                        Data = e.Data,
                        IsDirty = true,
                        LastAccessed = DateTime.Now
                    };
                    _currentCacheSize += e.Data.Length;
                }

                Console.WriteLine($"[DiskManager]: Byte file cached for write: {e.FilePath} ({e.Data.Length} bytes)");

                if (_currentCacheSize > MaxCacheSizeBytes)
                {
                    Console.WriteLine(
                        $"[DiskManager]: Cache size exceeded {MaxCacheSizeBytes / (1024 * 1024)} MB. Triggering flush.");
                    FlushFileCache();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiskManager]: Error caching byte file for write: {e.FilePath}, {ex.Message}");
            Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
        }
    }

    private static void OnRequestFileTextWrite(RequestFileWriteTextEvent e)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(e.Text);

            lock (FileLock)
            {
                if (FileCache.TryGetValue(e.FilePath, out var cachedFile))
                {
                    var oldSize = cachedFile.Data.Length;
                    cachedFile.Data = bytes;
                    cachedFile.IsDirty = true;
                    cachedFile.LastAccessed = DateTime.Now;
                    _currentCacheSize = _currentCacheSize - oldSize + bytes.Length;
                }
                else
                {
                    FileCache[e.FilePath] = new CachedFile
                    {
                        FilePath = e.FilePath,
                        Data = bytes,
                        IsDirty = true,
                        LastAccessed = DateTime.Now
                    };
                    _currentCacheSize += bytes.Length;
                }

                Console.WriteLine($"[DiskManager]: Text file cached for write: {e.FilePath} ({bytes.Length} bytes)");

                if (_currentCacheSize > MaxCacheSizeBytes)
                {
                    Console.WriteLine($"[DiskManager]: Cache size exceeded {MaxCacheSizeBytes / (1024 * 1024)} MB. Triggering flush.");
                    FlushFileCache();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiskManager]: Error caching text file for write: {e.FilePath}, {ex.Message}");
            Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
        }
    }

    // -------------------------------------------------------------------------
    // Read Handlers
    // -------------------------------------------------------------------------

    private static void OnRequestFileRead(RequestFileReadEvent e)
    {
        try
        {
            lock (FileLock)
            {
                if (FileCache.TryGetValue(e.FilePath, out var cachedFile))
                {
                    cachedFile.LastAccessed = DateTime.Now;
                    e.SetData(cachedFile.Data);
                    return;
                }

                if (File.Exists(e.FilePath))
                {
                    var data = File.ReadAllBytes(e.FilePath);

                    FileCache[e.FilePath] = new CachedFile
                    {
                        FilePath = e.FilePath,
                        Data = data,
                        IsDirty = false,
                        LastAccessed = DateTime.Now
                    };
                    _currentCacheSize += data.Length;

                    e.SetData(data);

                    if (_currentCacheSize > MaxCacheSizeBytes)
                        FlushFileCache();
                }
                else
                {
                    e.SetData(null);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiskManager]: Error reading byte file: {e.FilePath}, {ex.Message}");
            Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
            e.SetData(null);
        }
    }

    private static void OnRequestFileReadText(RequestFileReadTextEvent e)
    {
        try
        {
            lock (FileLock)
            {
                // Check cache first — text files are stored as UTF-8 bytes
                if (FileCache.TryGetValue(e.FilePath, out var cachedFile))
                {
                    cachedFile.LastAccessed = DateTime.Now;
                    e.SetText(Encoding.UTF8.GetString(cachedFile.Data));
                    return;
                }

                if (File.Exists(e.FilePath))
                {
                    var text = File.ReadAllText(e.FilePath, Encoding.UTF8);
                    var bytes = Encoding.UTF8.GetBytes(text);

                    FileCache[e.FilePath] = new CachedFile
                    {
                        FilePath = e.FilePath,
                        Data = bytes,
                        IsDirty = false,
                        LastAccessed = DateTime.Now
                    };
                    _currentCacheSize += bytes.Length;

                    e.SetText(text);

                    if (_currentCacheSize > MaxCacheSizeBytes)
                        FlushFileCache();
                }
                else
                {
                    e.SetText(null);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiskManager]: Error reading text file: {e.FilePath}, {ex.Message}");
            Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
            e.SetText(null);
        }
    }

    // -------------------------------------------------------------------------
    // Log Handler
    // -------------------------------------------------------------------------

    private static void OnLogWrite(LogEvents e)
    {
        try
        {
            LogBuffer.Enqueue(new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = e.Message
            });

            if (LogBuffer.Count >= MaxLogBufferEntries)
                FlushLogBuffer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiskManager]: Critical error buffering log entry: {ex.Message}");
            Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
        }
    }

    private static void OnSystemShutdown(SystemShutdownEvent e) => Flush();

    // -------------------------------------------------------------------------
    // Flush & Cache Management
    // -------------------------------------------------------------------------

    private static void FlushFileCache()
    {
        lock (FileLock)
        {
            var dirtyFiles = FileCache.Values.Where(f => f.IsDirty).ToList();

            if (dirtyFiles.Count == 0)
                return;

            foreach (var file in dirtyFiles)
            {
                try
                {
                    var directory = Path.GetDirectoryName(file.FilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    File.WriteAllBytes(file.FilePath, file.Data);
                    file.IsDirty = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DiskManager]: Error flushing file: {file.FilePath}, {ex.Message}");
                    Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
                }
            }

            TrimCache();
        }
    }

    private static void FlushLogBuffer()
    {
        lock (LogLock)
        {
            if (LogBuffer.IsEmpty)
                return;

            var logEntries = new List<LogEntry>();
            while (LogBuffer.TryDequeue(out var entry))
                logEntries.Add(entry);

            if (logEntries.Count == 0)
                return;

            try
            {
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);

                var logFileName = $"realm-{DateTime.Now:yyyy-MM-dd-HH-mm}.log";
                var logFilePath = Path.Combine(LogDirectory, logFileName);

                var sb = new StringBuilder();
                foreach (var entry in logEntries)
                    sb.AppendLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] {entry.Message}");

                File.AppendAllText(logFilePath, sb.ToString());

                Console.WriteLine($"[DiskManager]: Wrote {logEntries.Count} log entries to {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DiskManager]: Failed to flush log buffer: {ex.Message}");
                Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
            }
        }
    }

    private static void Flush()
    {
        try
        {
            FlushFileCache();
            FlushLogBuffer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiskManager]: Exception flushing buffers: {ex.Message}");
            Console.WriteLine($"[DiskManager]: StackTrace: {ex.StackTrace}");
        }
    }

    private static void TrimCache()
    {
        if (_currentCacheSize <= MaxCacheSizeBytes)
            return;

        var cleanFiles = FileCache.Values
            .Where(f => !f.IsDirty)
            .OrderBy(f => f.LastAccessed)
            .ToList();

        foreach (var file in cleanFiles)
        {
            if (_currentCacheSize <= MaxCacheSizeBytes)
                break;

            if (FileCache.TryRemove(file.FilePath, out _))
            {
                _currentCacheSize -= file.Data.Length;
                Console.WriteLine($"[DiskManager]: Evicted clean file from cache: {file.FilePath}");
            }
        }

        Console.WriteLine($"[DiskManager]: Cache trimmed. New size: {_currentCacheSize / 1024} KB");
    }

    // -------------------------------------------------------------------------
    // Public Diagnostics
    // -------------------------------------------------------------------------

    public static long GetCacheSizeBytes() => _currentCacheSize;
    public static int GetCachedFileCount() => FileCache.Count;
    public static int GetLogBufferCount() => LogBuffer.Count;

    public static int GetDirtyFileCount()
    {
        lock (FileLock)
        {
            return FileCache.Values.Count(f => f.IsDirty);
        }
    }

    // -------------------------------------------------------------------------
    // Shutdown
    // -------------------------------------------------------------------------

    public static void Shutdown()
    {
        if (!_initialized)
            return;

        Flush();

        ClockManager.StopClock("DiskManager");
        ClockManager.UnregisterClock("DiskManager");
        _clock?.OnTick -= OnTick;
        _clock = null;
        
        EventManager.Unsubscribe<RequestFileWriteByteEvent>(OnRequestFileByteWrite);
        EventManager.Unsubscribe<RequestFileWriteTextEvent>(OnRequestFileTextWrite);
        EventManager.Unsubscribe<RequestFileReadEvent>(OnRequestFileRead);
        EventManager.Unsubscribe<RequestFileReadTextEvent>(OnRequestFileReadText);
        EventManager.Unsubscribe<LogEvents>(OnLogWrite);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);

        _initialized = false;
        Console.WriteLine("[DiskManager]: Shutdown.");
    }
}

/*
 *------------------------------------------------------------
 * (DiskManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */