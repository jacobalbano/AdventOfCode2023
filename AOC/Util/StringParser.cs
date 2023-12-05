using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AOC.Util;

[DebuggerDisplay("{str.Substring(cursor)}")]
public class StringParser
{
    public bool HasMaterial => cursor < str.Length;

    public StringParser(string input)
    {
        str = input;
    }

    public char Peek()
    {
        return str[cursor];
    }

    public int ReadInt()
    {
        if (!TryReadInt(out var result))
            throw new Exception("Failed to find integer in material");

        return result;
    }

    public string ReadUntil(string term, bool skip)
    {
        int start = cursor,
            index = str.IndexOf(term, cursor);
        if (index < 0)
            throw new Exception("Failed to find search term in material");

        if (skip)
            cursor = index + term.Length;
        else
            cursor = index;

        return str[start..index];
    }

    public string ReadUntil(char term, bool skip)
    {
        int start = cursor, index = cursor;
        while (index < str.Length && str[index] != term)
            index++;

        if (index >= str.Length)
            throw new Exception("Failed to find search term in material");

        if (skip)
            cursor = index + 1;
        else
            cursor = index;

        return str[start..index];
    }

    public bool TryReadInt(out int result)
    {
        int start = cursor;
        while (cursor < str.Length)
        {
            if (!char.IsNumber(str[cursor]))
                break;

            cursor++;
        }

        return int.TryParse(str.AsSpan(start, cursor - start), out result);
    }

    public StringParser Skip(int length)
    {
        cursor += length;
        return this;
    }

    public StringParser SkipUntil(char c, bool goPast)
    {
        ReadUntil(c, goPast);
        return this;
    }

    public StringParser SkipExact(string skip)
    {
        int j = 0;
        while (cursor < str.Length && j < skip.Length)
        {
            if (str[cursor] != skip[j++])
                throw new Exception("SkipExact encountered a mismatch");

            cursor++;
        }

        if (j < skip.Length - 1)
            throw new Exception("SkipExact expended available material before completing pattern");

        return this;
    }

    public StringParser SkipAny(string chars)
    {
        while (cursor < str.Length)
        {
            if (!chars.Any(x => x == str[cursor]))
                break;
            
            cursor++;
        }

        return this;
    }

    public StringParser SkipWhile(Func<char, bool> predicate)
    {
        while (cursor < str.Length && predicate(str[cursor]))
            cursor++;

        return this;
    }

    public char ReadChar()
    {
        return str[cursor++];
    }

    public bool TryReadChar(out char c)
    {
        c = default;
        if (!HasMaterial) return false;
        c = ReadChar();
        return true;
    }

    public string ReadRemainder()
    {
        var result = str[cursor..];
        cursor = str.Length;
        return result;
    }

    private readonly string str;
    private int cursor = 0;
}
