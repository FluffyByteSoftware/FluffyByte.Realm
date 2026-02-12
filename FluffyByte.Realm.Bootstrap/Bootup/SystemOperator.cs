/*
 * (SystemOperator.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:56:28 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Networking.LoginServer;
using FluffyByte.Realm.Networking.ServerCore;
using FluffyByte.Realm.Networking.ServerCore.Clients;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Disk;
using FluffyByte.Realm.Tools.Heartbeats;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class SystemOperator
{
    public static string ServerName { get; private set; } = "Taerin's Whisper";

    public static void InitializeSystem()
    {
        DiskManager.Initialize();

        var realmServer = new RealmServer(
            "Taerin's Whisper",
            10,
            "76.130.204.118",
            9997);
        
        RealmManager.Initialize(realmServer);
        ClientManager.Initialize();
        
        ServerName = "Taerin's Whisper";
        
    }

    public static void StartSystem()
    {
        EventManager.Publish(new SystemStartupEvent());
    }

    public static void ShutdownSystem()
    {
        EventManager.Publish(new SystemShutdownEvent());
    }

    public static string Version => "0.0.1";
}

/*
 *------------------------------------------------------------
 * (SystemOperator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */