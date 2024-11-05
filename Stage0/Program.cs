using System.Diagnostics;

partial class Program
{
    private static void Main(string[] args)
    {
        Welcome1454();
        Console.ReadKey();
        Welcome3700();
    }
    static partial void Welcome3700();
    private static void Welcome1454()
    {
        Console.Write("Enter your name:");
        string name = Console.ReadLine();
        Console.WriteLine("{0}, welcome to my first console application", name);
    }
}