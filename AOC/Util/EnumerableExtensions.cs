using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

public static class EnumerableExtensions
{
    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> self, Func<T, bool> predicate)
    {
        foreach (var item in self)
        {
            yield return item;
            if (predicate(item))
                break;
        }
    }

    public static IEnumerable<T> Iterate<T>(T seed, Func<T, T> iterate)
    {
        yield return seed;
        while (true)
            yield return seed = iterate(seed);
    }

    public static IEnumerable<IReadOnlyList<T>> Pivot<T>(this IEnumerable<IEnumerable<T>> self)
    {
        var enums = self.Select(x => x.GetEnumerator())
            .ToList();

        while (true)
        {
            if (!enums.All(x => x.MoveNext()))
                yield break;

            var result = new T[enums.Count];
            for (int i = 0; i < enums.Count; i++)
                result[i] = enums[i].Current;

            yield return result;
        }
    }

    public static IEnumerable<T> RepeatInfinitely<T>(this IEnumerable<T> self)
    {
        for (var e = self.GetEnumerator(); ;)
        {
            if (e.MoveNext())
                yield return e.Current;
            else
                e.Reset();
        }
    }

    public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e, bool started)
    {
        if (!started) e.MoveNext();
        do
        {
            yield return e.Current;
        } while (e.MoveNext());
    }

    public static IEnumerable<long> RangeL(long start, long length)
    {
        for (long i = 0; i < length; ++i)
            yield return start + i;
    }

    public static int[] ToArray(this Range range)
    {
        int start = range.Start.Value,
            end = range.End.Value,
            span = end - start,
            sign = Math.Sign(span);

        var result = new int[Math.Abs(span)];
        for (int i = 0; i < result.Length; i++)
            result[i] = start + i * sign;

        return result;
    }

    public static int FindIndex<T>(this T[] self, Predicate<T> match)
    {
        for (int i = 0; i < self.Length; i++)
            if (match(self[i])) return i;

        return -1;
    }

    public static IEnumerable<IEnumerable<T>> PartitionBy<T>(this IEnumerable<T> self, Func<T, bool> delimit)
    {
        using var e = self.GetEnumerator();
        while (e.MoveNext())
            yield return Inner();

        IEnumerable<T> Inner()
        {
            do
            {
                if (delimit(e.Current))
                    yield break;

                yield return e.Current;
            } while (e.MoveNext());
        }
    }

    public static IEnumerable<(T, T)> Lag<T>(this IEnumerable<T> self)
    {
        using var e = self.GetEnumerator();
        if (!e.MoveNext())
            yield break;

        T first = e.Current;
        while (e.MoveNext())
            yield return (first, first = e.Current);
    }

    public static IEnumerable<T[]> ChunkBy<T>(this IEnumerable<T> self, int chunkSize)
    {
        using var e = self.GetEnumerator();
        for (bool go = e.MoveNext(); go;)
        {
            var result = new T[chunkSize];
            for (int i = 0; i < chunkSize; i++)
            {
                result[i] = e.Current;
                if (!(go = e.MoveNext()))
                    break;
            }

            yield return result;
        }
    }

    public static IEnumerable<T[]> WindowBy<T>(this IEnumerable<T> self, int windowSize)
    {
        using var e = self.GetEnumerator();
        bool go = e.MoveNext();
        if (!go) yield break;
        
        //  make first window
        var result = new T[windowSize];
        for (int i = 0; i < windowSize; i++)
        {
            result[i] = e.Current;
            if (!(go = e.MoveNext()))
                break;
        }

        yield return result;

        //  slide it
        while (go)
        {
            var last = result;
            result = new T[windowSize];
            for (int i = 1; i < windowSize; i++)
                result[i - 1] = last[i];

            result[windowSize - 1] = e.Current;
            yield return result;
            go = e.MoveNext();
        }
    }

    public static IEnumerable<T> As<T>(this IEnumerable untyped)
    {
        foreach (var x in untyped)
            yield return (T)x;
    }

    public static IEnumerable<(T, Action)> AsRemovable<T>(this List<T> self)
    {
        for (int i = 0; i < self.Count;)
        {
            bool pass = false;
            yield return (self[i], () => pass = true);

            if (pass) self.RemoveAt(i);
            else ++i;
        }
    }

    public delegate (bool, TOut) SelectWhereSelector<TIn, TOut>(TIn input);

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> self, SelectWhereSelector<TIn, TOut> selector)
    {
        foreach (var x in self)
        {
            var (success, result) = selector(x);
            if (success) yield return result;
        }
    }

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> self, Func<TIn, (TOut, bool)> selector)
    {
        foreach (var x in self)
        {
            var (success, result) = selector(x);
            if (result) yield return success;
        }
    }

    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
        foreach (var sequence in sequences)
        {
            var localSequence = sequence;
            result = result.SelectMany(
              _ => localSequence,
              (seq, item) => seq.Concat(new[] { item })
            );
        }
        return result;
    }

    public static TResult[] MultiMax<T, TResult>(this IEnumerable<T> self, params Func<T, TResult>[] selectors)
    {
        using var e = self.GetEnumerator();
        var comparer = Comparer<TResult>.Default;

        var result = new TResult[selectors.Length];
        if (!e.MoveNext()) return result;

        for (int i = 0; i < selectors.Length; i++)
            result[i] = selectors[i](e.Current);

        while (e.MoveNext())
        {
            for (int i = 0; i < selectors.Length; i++)
            {
                var (x, y) = (result[i], selectors[i](e.Current));
                if (comparer.Compare(x, y) < 0)
                    result[i] = y;
            }
        }

        return result;
    }
}
