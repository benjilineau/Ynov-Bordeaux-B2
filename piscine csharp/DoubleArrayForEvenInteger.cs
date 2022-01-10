using System;
namespace CSharpDiscovery.Quest01
{
    public class DoubleArrayForEvenInteger_Exercice
    {
         public static int[] DoubleArrayForEvenInteger(int[] inputTab)
        {
          
           foreach (int i in inputTab){
               int index = Array.IndexOf(inputTab, i);
               if (i % 2 == 0){
                   inputTab[index] = (i);
               } else {
                   inputTab[index] = (i*2);
               }
           }
           return inputTab;
        }
    }
}