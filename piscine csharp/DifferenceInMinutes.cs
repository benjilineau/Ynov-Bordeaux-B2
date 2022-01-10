using System;

namespace CSharpDiscovery.Quest02
{
    public class DifferenceInMinutes_Exercice
    {
        public static double DifferenceInMinutes(DateTime start, DateTime end)
        {
            TimeSpan next = end - start;
            return next.TotalMinutes;
        }
    }
}