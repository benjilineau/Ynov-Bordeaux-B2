using System;
namespace CSharpDiscovery.Quest04
{
    public class HybridCar : Car,IElectricCar,IThermalCar
    {
        private int _batteryLevel = 100;
        public int BatteryLevel 
        { 
            get => _batteryLevel; 
            set => _batteryLevel = value; 
        }
        private int _fuelLevel = 100;
        public int FuelLevel 
        { 
            get => _fuelLevel; 
            set => _fuelLevel = value; 
        }
        public HybridCar() : base(){}
        public HybridCar(String model, String brand, String color, int currentSpeed = 0) : base(model, brand, color, currentSpeed){}
        public  int GetFuelLevel() { return FuelLevel; }
        public  int GetBatteryLevel() { return BatteryLevel; }
        public void FillUp() {
            FuelLevel = 100;
        }
        public  void Recharge(){
            BatteryLevel = 100;
        }
        public override string ToString(){
            return $"{Color} {Brand} {Model}, Battery: {BatteryLevel}%, Fuel: {FuelLevel}%";
        }

    }
}