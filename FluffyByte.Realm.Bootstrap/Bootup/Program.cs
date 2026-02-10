using System.Text;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Disk;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello World");
        
        var writeData = Encoding.UTF8.GetBytes("Hello World");

        SystemOperator.InitializeSystem();
        
        var fileWrite = new RequestFileWriteEvent()
        {
            Data = writeData,
            FilePath = @"E:\test.txt"
        };

        EventManager.Publish(fileWrite);
        
        SystemOperator.ShutdownSystem();
        
    }
}