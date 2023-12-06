using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StringExtensions
{
    public static string[] SplitSpaces(this string str)
    {
        return str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    public static IEnumerable<string> ToLines(this string input)
    {
        using StringReader sr = new StringReader(input);
        string line = sr.ReadLine(), nextLine = null;
        if (string.IsNullOrEmpty(line))
            nextLine = sr.ReadLine();
        else nextLine = line;

        while ((line = sr.ReadLine()) != null)
        {
            yield return nextLine;
            nextLine = line;
        }

        if (!string.IsNullOrEmpty(nextLine))
            yield return nextLine;
    }

    public static IEnumerable<string> CSV(this string input)
    {
        return input.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());
    }
}
