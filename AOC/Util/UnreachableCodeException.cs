using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2021.Util
{
    public class UnreachableCodeException : Exception
    {
        public UnreachableCodeException() : base("Encountered a code path that should be unreachable")
        {

        }
    }
}
