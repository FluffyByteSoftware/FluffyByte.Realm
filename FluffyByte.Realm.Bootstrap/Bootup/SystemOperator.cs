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
    
    public static void InitializeSystem()
    {
        Log.Debug("Initializing system components");
        
        DiskManager.Initialize();

        Log.Debug($"Initialization of system components completed.");
    }

    public static void StartSystem()
    {
        
        Log.Debug("Starting system components");
        
        Log.Debug($"Finished starting system components.");
    }

    public static void ShutdownSystem()
    {
        Log.Debug($"Beginning system shutdown...");

        EventManager.Publish(new SystemShutdownEvent());
    }
}

/*
 *------------------------------------------------------------
 * (SystemOperator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */