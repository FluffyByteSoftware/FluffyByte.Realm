namespace FluffyByte.Realm.Bootstrap.Bootup;

public static class Program
{
    public static void Main()
    {
        SystemOperator.InitializeSystem();
     
        SystemOperator.StartSystem();
        
        Console.ReadLine();
        
        SystemOperator.ShutdownSystem();
    }
}