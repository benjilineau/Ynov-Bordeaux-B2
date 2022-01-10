using System;

namespace CSharpDiscovery.Quest02
{
    public class FromStringToDateTime_Exercice
    {
        public static DateTime FromStringToDateTime(string dateStr)
        {
            return DateTime.ParseExact(dateStr,"dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}