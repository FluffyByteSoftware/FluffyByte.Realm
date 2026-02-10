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
    private const int TicksBetweenFlushes = 9000; // 15 minutes at 10 ticks/sec (100 ms tick)
    private const int MaxLogBufferEntries = 1000;

    private static long _currentCacheSize;
    private static long _lastFlushTick;
    private static bool _initialized;
    
    public static void Initialize()
    {
        if (_initialized)
            return;

        EventManager.Subscribe<RequestFileWriteEvent>(OnRequestFileWrite);
        EventManager.Subscribe<RequestFileReadEvent>(OnRequestFileRead);
        EventManager.Subscribe<LogEvents>(OnLogWrite);
        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Subscribe<TickEvent>(OnTick);
        
        _initialized = true;
        ClockManager.RegisterClock("DiskManager", 1000);

        ClockManager.StartClock("DiskManager");
        
        Console.WriteLine($"DiskManager initialized.");
    }

    private static void OnTick(TickEvent e)
    {
        if (e.ClockName != "DiskManager")
            return;

        if (e.TickNumber - _lastFlushTick >= TicksBetweenFlushes)
        {
            Console.WriteLine("Periodic flush interval reached. Flushing buffers.");
            
            Flush();
            
            _lastFlushTick = e.TickNumber;
        }
    }

    private static void OnRequestFileWrite(RequestFileWriteEvent e)
    {
        try
        {
            lock (FileLock)
            {
                // Check if the file already exists in the cache
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
                    cachedFile = new CachedFile
                    {
                        FilePath = e.FilePath,
                        Data = e.Data,
                        IsDirty = true,
                        LastAccessed = DateTime.UtcNow
                    };

                    FileCache[e.FilePath] = cachedFile;
                    _currentCacheSize += e.Data.Length;
                }

                Console.WriteLine($"File cached for write: {e.FilePath} ({e.Data.Length} bytes)");

                if (_currentCacheSize > MaxCacheSizeBytes)
                {
                    Console.WriteLine($"Cache size exceeded {MaxCacheSizeBytes / (1024 * 1024)} MB. Triggering flush.");
                    FlushFileCache();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error caching file for write: {e.FilePath}, {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

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

                // Not in cache, read from disk
                if (File.Exists(e.FilePath))
                {
                    var data = File.ReadAllBytes(e.FilePath);

                    var newCachedFile = new CachedFile
                    {
                        FilePath = e.FilePath,
                        Data = data,
                        IsDirty = false,
                        LastAccessed = DateTime.Now
                    };

                    FileCache[e.FilePath] = newCachedFile;
                    _currentCacheSize += data.Length;

                    e.SetData(data);

                    if (_currentCacheSize > MaxCacheSizeBytes)
                    {
                        FlushFileCache();
                    }
                }
                else
                {
                    e.SetData(null);
                }
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {e.FilePath}, {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            
            e.SetData(null);
        }
    }

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
            {
                FlushLogBuffer();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Error buffering the log entry: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

    private static void OnSystemShutdown(SystemShutdownEvent e)
    {
        Flush();
    }

    private static void FlushFileCache()
    {
        lock (FileLock)
        {
            var dirtyFiles = FileCache.Values.Where(f => f.IsDirty).ToList();

            if (dirtyFiles.Count == 0)
            {
                return;
            }
            
            foreach (var file in dirtyFiles)
            {
                try
                {
                    var directory = Path.GetDirectoryName(file.FilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllBytes(file.FilePath, file.Data);
                    file.IsDirty = false;
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error flushing file: {file.FilePath}, {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
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
            {
                return;
            }

            var logEntries = new List<LogEntry>();
            
            while (LogBuffer.TryDequeue(out var entry))
            {
                logEntries.Add(entry);
            }

            if (logEntries.Count == 0)
            {
                return;
            }

            try
            {
                var logDirectory = @$"E:\Temp\Server\logs";
                
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logFileName = $"realm-{DateTime.Now:yyyy-MM-dd-hh-mm}.log";
                var logFilePath = Path.Combine(logDirectory, logFileName);

                var sb = new StringBuilder();
                
                foreach (var entry in logEntries)
                {
                    sb.AppendLine(entry.Message);
                }

                Console.WriteLine($"SB: {sb.ToString()}");

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
            Console.WriteLine($"Exception flushing buffers: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

    private static void TrimCache()
    {
        if (_currentCacheSize <= MaxCacheSizeBytes)
        {
            return;
        }

        var cleanFiles = FileCache.Values
            .Where(f => !f.IsDirty)
            .OrderBy(f => f.LastAccessed)
            .ToList();

        foreach (var file in cleanFiles)
        {
            if (_currentCacheSize <= MaxCacheSizeBytes)
            {
                break;
            }

            if (FileCache.TryRemove(file.FilePath, out _))
            {
                _currentCacheSize -= file.Data.Length;
                Console.WriteLine($"[DiskManager]: Evicted clean file from cache: {file.FilePath}");
            }
        }

        Console.WriteLine($"[DiskManager]: Cached Trimmed. New Size: {_currentCacheSize / 1024} KB");
    }

    public static long GetCacheSizeBytes() => _currentCacheSize;
    public static int GetCachedFileCount() => FileCache.Count;

    public static int GetDirtyFileCount()
    {
        lock (FileLock)
        {
            return FileCache.Values.Count(f => f.IsDirty);
        }
    }

    public static int GetLogBufferCount() => LogBuffer.Count;

    public static void Shutdown()
    {
        if (!_initialized)
            return;

        Flush();
        
        ClockManager.StopClock("DiskManager");
        ClockManager.UnregisterClock("DiskManager");

        EventManager.Unsubscribe<RequestFileWriteEvent>(OnRequestFileWrite);
        EventManager.Unsubscribe<RequestFileReadEvent>(OnRequestFileRead);
        EventManager.Unsubscribe<LogEvents>(OnLogWrite);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Unsubscribe<TickEvent>(OnTick);

        _initialized = false;
        Console.WriteLine($"[DiskManager]: Shutdown.");
    }
}

/*
 *------------------------------------------------------------
 * (DiskManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */