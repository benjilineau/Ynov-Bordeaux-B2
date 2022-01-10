using CSharpDiscovery.Quest04;
using System;

namespace TestCSharp
{
    class Program
    {
        static void Main(string[] str)
        {
            var i8 = new HybridCar("i8", "BMW", "White");
            i8.BatteryLevel = 60;
            i8.FuelLevel = 80;
            Vehicule.WhoIsHere();

            i8.Recharge();
            i8.FillUp();
            Console.WriteLine(i8.GetBatteryLevel());
            Console.WriteLine(i8.GetFuelLevel());
        }
    }
}