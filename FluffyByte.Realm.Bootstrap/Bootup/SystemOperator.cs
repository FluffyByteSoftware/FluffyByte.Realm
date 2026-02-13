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

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class SystemOperator
{
    
    public static void InitializeSystem()
    {
        DiskManager.Initialize();
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