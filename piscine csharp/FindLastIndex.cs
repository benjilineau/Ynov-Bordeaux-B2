using System;
namespace CSharpDiscovery.Quest02
{
    public class FindLastIndex_Exercice
    {
        public static int? FindLastIndex(int[] tab, int a)
        {
            if(!ContainsThisValue_Exercice.ContainsThisValue(tab,a)){
                return null;
            }
            return Array.LastIndexOf(tab,a);
        }
    }
}