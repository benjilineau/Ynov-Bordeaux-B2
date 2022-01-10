using CSharpDiscovery.Models;
using System;

namespace CSharpDiscovery.Quest01
{
    public class NextTrafficLightColor_Exercice
    {
        public static TrafficLightColor GetNextTrafficLightColor(TrafficLightColor currentColor)
        {
            var next = (TrafficLightColor)(Convert.ToInt32(currentColor)+1);
            if ((int)currentColor == 0){
                return (TrafficLightColor)0;
            }
            if ((int)next == 4){
                return (TrafficLightColor)1;
            }
            return next;
        }
    }
}