/*
 * (SystemOperator.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:56:28 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Brains;
using FluffyByte.Realm.Networking.Accounting;
using FluffyByte.Realm.Networking.Server;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Disk;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class SystemOperator
{
    public static void InitializeSystem()
    {
        DiskManager.Initialize();
        RealmManager.Initialize();
        PacketManager.Initialize();
        AccountManager.Initialize();
        ClientManager.Initialize();
        GameDirector.Initialize();
        LoginHandler.Initialize();
    }

    public static void StartSystem()
    {
        EventManager.Publish(new SystemStartupEvent());
    }

    public static void ShutdownSystem()
    {
        EventManager.Publish(new SystemShutdownEvent());
    }
}

/*
 *------------------------------------------------------------
 * (SystemOperator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */