using System.Linq;
namespace CSharpDiscovery.Quest02
{
    public class ContainsThisValue_Exercice
    {
        public static bool ContainsThisValue(int[] tab, int value)
        {
           if (tab == null){
               return false;
           }
           return tab.Contains(value);
        }
    }
}