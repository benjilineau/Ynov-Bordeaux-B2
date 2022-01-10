using System;
namespace CSharpDiscovery.Quest04 {
    public class Truck : Vehicule {
        public int Tonnage {
            get;set;
        }
        public Truck() : base () {
            Tonnage = 0;
        }
        public Truck(int tonnage, string Brand, string Color, int CurrentSpeed = 0) : base (Brand, Color, CurrentSpeed) {
            Tonnage = tonnage;
        }

        public override void Accelerate(int Speed) {
            if((CurrentSpeed + Speed) > 100){
                CurrentSpeed = 100;
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
            string res = $"{Color} {Brand} {Tonnage}T Truck";
            return res;
        }
    }
}