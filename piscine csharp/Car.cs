using System;
namespace CSharpDiscovery.Quest04 {
    public class Car : Vehicule {
        public string Model {
            get;set;
        }
        public Car() : base () {
            Model = "Unknown";
        }
        public Car(string model, string Brand, string Color, int CurrentSpeed = 0) : base (Brand, Color, CurrentSpeed) {
            Model = model;
        }

        public override void Accelerate(int Speed) {
            if((CurrentSpeed + Speed) > 180){
                CurrentSpeed = 180;
            }else if((CurrentSpeed + Speed) < 180){
                CurrentSpeed += Speed;
            }
        }

        public override void Brake(int BrakePower){
            if((CurrentSpeed - BrakePower) < 0){
                CurrentSpeed = 0;
            } else if((CurrentSpeed - BrakePower)>0){
                CurrentSpeed -= BrakePower;
            }
        }

        public override string ToString() {
            string res = $"{Color} {Brand} {Model}";
            return res;
        }
    }
}