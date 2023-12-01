using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


internal static class RegexExtensions
{
    public static IEnumerable<string> OverlappingMatches(this Regex regex, string line)
    {
        int start = 0;
        while (true)
        {
            var match = regex.Match(line, start);
            if (!match.Success) break;
            yield return match.Value;
            start = match.Index + 1;
        }
    }
}
