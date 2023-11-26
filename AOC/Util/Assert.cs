using AdventOfCode2021.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class Assert
{
    public static void ArraysMatch<T>(T[] expected, T[] actual)
    {
        if (expected.Length != actual.Length)
            throw new AssertionFailure("expected and actual had different lengths");

        for (int i = 0; i < expected.Length; i++)
            IsTrue(Equals(expected[i], actual[i]), $"expected and actual differ at position {i} ({expected[i]}) vs {actual[i]})");
    }

    public static void IsFalse(bool condition, [CallerArgumentExpression("condition")] string message = null)
    {
        IsTrue(!condition, message);
    }

    public static void IsTrue(bool condition, [CallerArgumentExpression("condition")] string message = null)
    {
        if (!condition)
            throw new AssertionFailure(message);
    }

    public static void AreEqual<T>(T expected, T actual, string message)
    {
        if (!Equals(expected, actual))
            throw new AssertionFailure(message);
    }

    public static void AreEqual<T>(T expected, T actual)
    {
        if (!Equals(expected, actual))
            throw new AssertionFailure($"Expected: {expected}, actual: {actual}");
    }

    public static void Unreachable()
    {
        throw new UnreachableCodeException();
    }

    public class AssertionFailure : Exception
    {
        public AssertionFailure(string message) : base(message)
        {

        }
    }
}