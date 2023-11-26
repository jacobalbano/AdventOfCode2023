using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> self, IEnumerable<T> range)
    {
        foreach (var x in range)
            self.Add(x);
    }

    public static void AddRange<T>(this HashSet<T> self, params T[] range)
    {
        foreach (var x in range)
            self.Add(x);
    }
}