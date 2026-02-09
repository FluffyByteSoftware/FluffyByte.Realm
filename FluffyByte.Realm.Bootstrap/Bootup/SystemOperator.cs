/*
 * (SystemOperator.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:56:28 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Disk;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class SystemOperator
{
    public static CancellationToken ShutdownToken { get; private set; }

    private static CancellationTokenSource? _shutdownSource = null;
    
    public static void InitializeSystem()
    {
        Log.Debug("Initializing system components");
        
        _shutdownSource = new CancellationTokenSource();
        ShutdownToken = _shutdownSource.Token;
        
        DiskManager.Initialize();

        Log.Debug($"Initialization of system components completed.");
    }

    public static void StartSystem()
    {
        if (_shutdownSource is null || _shutdownSource.IsCancellationRequested)
            return;
        
        Log.Debug("Starting system components");
        
        Log.Debug($"Finished starting system components.");
    }

    public static void ShutdownSystem()
    {
        if (_shutdownSource is null)
            return;
        
        Log.Debug($"Beginning system shutdown...");

        _shutdownSource.Cancel();

        EventManager.Publish(new SystemShutdownEvent());
    }
}

/*
 *------------------------------------------------------------
 * (SystemOperator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */