/*
 * (SystemOperator.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:56:28 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.Accounts;
using FluffyByte.Realm.Networking.Clients;
using FluffyByte.Realm.Networking.ServerCore;
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
        RealmClientManager.Initialize();
        
        var realmServer = new RealmServer(
            "Taerin's Whisper", 
            "10.0.0.84", 
            9997, 
            10, 
            "Test");

        Sentinel.Initialize(realmServer);
        RealmClientManager.Initialize();
        AccountManager.Initialize();
        
        Log.Debug($"Initialization of system components completed.");
    }

    public static void StartSystem()
    {
        
        Log.Debug("Starting system components");

        Sentinel.Start();
        
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