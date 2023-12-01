using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class LoggerExtensions
{
    public static T Print<T>(this T value, ILogger logger)
    {
        logger.LogLine(value);
        return value;
    }
}
