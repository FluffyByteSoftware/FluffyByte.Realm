/*
 * (DiskManager.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:58:53 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Heartbeats;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Tools.Disk;

public static class DiskManager
{
    private static readonly ConcurrentDictionary<string, CachedFile> _fileCache = [];
    private static readonly ConcurrentQueue<LogEntry> _logBuffer = [];

    private static readonly Lock _fileLock = new Lock();
    private static readonly Lock _logLock = new Lock();

    private const int MaxCacheSizeBytes = 50 * 1024 * 1024; // 50 MB
    private const int TicksBetweenFlushes = 9000; // 15 minutes at 10 ticks/sec (100 ms tick)
    private const int MaxLogBufferEntries = 1000;

    private static long _currentCacheSize = 0;
    private static long _lastFlushTick = 0;
    private static bool _initialized = false;
    
    public static void Initialize()
    {
        if (_initialized)
            return;

        EventManager.Subscribe<RequestFileWriteEvent>(OnRequestFileWrite);
        EventManager.Subscribe<RequestFileReadEvent>(OnRequestFileRead);
        EventManager.Subscribe<LogWriteEvent>(OnLogWrite);
        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Subscribe<TickEvent>(OnTick);
        
        _initialized = true;

        Log.Debug($"DiskManager initialized.");
    }

    private static void OnTick(TickEvent e)
    {
        if (e.ClockName != "DiskManager")
            return;

        if (e.TickNumber - _lastFlushTick >= TicksBetweenFlushes)
        {
            Log.Debug("Periodic flush interval reached. Flushing buffers.");
            
            Flush();
            
            _lastFlushTick = e.TickNumber;
        }
    }

    private static void OnRequestFileWrite(RequestFileWriteEvent e)
    {
        try
        {
            lock (_fileLock)
            {
                // Check if file already exists in cache
                if (_fileCache.TryGetValue(e.FilePath, out var cachedFile))
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

                    _fileCache[e.FilePath] = cachedFile;
                    _currentCacheSize += e.Data.Length;
                }

                Log.Debug($"File cached for write: {e.FilePath} ({e.Data.Length} bytes)");

                if (_currentCacheSize > MaxCacheSizeBytes)
                {
                    Log.Info($"Cache size exceeded {MaxCacheSizeBytes / (1024 * 1024)} MB. Triggering flush.");
                    FlushFileCache();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error caching file write: {e.FilePath}", ex);
        }
    }

    private static void OnRequestFileRead(RequestFileReadEvent e)
    {
        try
        {
            lock (_fileLock)
            {
                if (_fileCache.TryGetValue(e.FilePath, out var cachedFile))
                {
                    cachedFile.LastAccessed = DateTime.Now;
                    
                    Log.Debug($"File read from cache: {e.FilePath}");

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

                    _fileCache[e.FilePath] = newCachedFile;
                    _currentCacheSize += data.Length;

                    Log.Debug($"File read from disk and cached: {e.FilePath} ({data.Length} bytes)");
                    e.SetData(data);

                    if (_currentCacheSize > MaxCacheSizeBytes)
                    {
                        FlushFileCache();
                    }
                }
                else
                {
                    Log.Warn($"File not found: {e.FilePath}");
                    e.SetData(null);
                }
            }
            
        }
        catch (Exception ex)
        {
            Log.Error($"Error reading file: {e.FilePath}", ex);
            e.SetData(null);
        }
    }

    private static void OnLogWrite(LogWriteEvent e)
    {
        try
        {
            _logBuffer.Enqueue(new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = e.Message
            });

            if (_logBuffer.Count >= MaxLogBufferEntries)
            {
                Log.Debug($"Log buffer reached {MaxLogBufferEntries} entries. Flushing...");
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
        Log.Info($"System shutdown detected. Performing final flush.");
        Flush();
    }

    private static void FlushFileCache()
    {
        lock (_fileLock)
        {
            var dirtyFiles = _fileCache.Values.Where(f => f.IsDirty).ToList();

            if (dirtyFiles.Count == 0)
            {
                Log.Debug($"No dirty files to flush.");
                return;
            }

            Log.Info($"Flushing {dirtyFiles.Count} dirty file(s) to disk.");

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

                    Log.Debug($"Flushed file to disk: {file.FilePath}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to flush file: {file.FilePath}", ex);
                }
            }

            TrimCache();
        }
    }

    private static void FlushLogBuffer()
    {
        lock (_logLock)
        {
            if (_logBuffer.IsEmpty)
            {
                return;
            }

            var logEntries = new List<LogEntry>();
            
            while (_logBuffer.TryDequeue(out var entry))
            {
                logEntries.Add(entry);
            }

            if (logEntries.Count == 0)
            {
                return;
            }

            try
            {
                var logDirectory = "logs";
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logFileName = $"realm-{DateTime.Now:yyyy-MM-dd-hh-mm.log}";
                var logFilePath = Path.Combine(logDirectory, logFileName);

                var sb = new StringBuilder();
                foreach (var entry in logEntries)
                {
                    sb.AppendLine(entry.Message);
                }

                File.AppendAllText(logFilePath, sb.ToString());
                
                Console.WriteLine($"Wrote {logEntries.Count} log entries to {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DiskManager] Failed to flush log buffer: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }
    }

    public static void Flush()
    {
        FlushFileCache();
        FlushLogBuffer();
    }

    private static void TrimCache()
    {
        if (_currentCacheSize <= MaxCacheSizeBytes)
        {
            return;
        }

        var cleanFiles = _fileCache.Values
            .Where(f => !f.IsDirty)
            .OrderBy(f => f.LastAccessed)
            .ToList();

        foreach (var file in cleanFiles)
        {
            if (_currentCacheSize <= MaxCacheSizeBytes)
            {
                break;
            }

            if (_fileCache.TryRemove(file.FilePath, out _))
            {
                _currentCacheSize -= file.Data.Length;
                Log.Debug($"[DiskManager]: Evicted clean file from cache: {file.FilePath}");
            }
        }

        Log.Info($"[DiskManager]: Cached Trimmed. New Size: {_currentCacheSize / 1024} KB");
    }

    public static long GetCacheSizeBytes() => _currentCacheSize;
    public static int GetCachedFileCount() => _fileCache.Count;

    public static int GetDirtyFileCount()
    {
        lock (_fileLock)
        {
            return _fileCache.Values.Count(f => f.IsDirty);
        }
    }

    public static int GetLogBufferCount() => _logBuffer.Count;

    public static void Shutdown()
    {
        if (!_initialized)
            return;

        Flush();

        EventManager.Unsubscribe<RequestFileWriteEvent>(OnRequestFileWrite);
        EventManager.Unsubscribe<RequestFileReadEvent>(OnRequestFileRead);
        EventManager.Unsubscribe<LogWriteEvent>(OnLogWrite);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
        EventManager.Unsubscribe<TickEvent>(OnTick);

        _initialized = false;
        Log.Info($"[DiskManager]: Shutdown.");
    }
}

/*
 *------------------------------------------------------------
 * (DiskManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */