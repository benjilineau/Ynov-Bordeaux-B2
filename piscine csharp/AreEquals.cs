using System.Linq;
namespace CSharpDiscovery.Quest02
{
    public class AreEquals_Exercice
    {
        public static bool AreEquals(int[] tab1, int[] tab2)
        {
            if (Enumerable.SequenceEqual(tab1,tab2)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}