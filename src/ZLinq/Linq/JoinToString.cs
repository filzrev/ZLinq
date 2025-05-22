using System.Globalization;
using System.Text;
using ZLinq.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace ZLinq;

partial class ValueEnumerableExtensions
{
    const int StackallocCharBufferSizeLimit = 256;

    public static string JoinToString<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, string separator)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, char separator)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ReadOnlySpan<char> separator)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (separator.Length == 0)
        {
            if (e.TryGetSpan(out var span))
            {
                if (span.Length == 0) return "";
                if (span.Length == 1) return span[0]!.ToString() ?? "";

                var result = new DefaultInterpolatedStringHandler(0, 0, CultureInfo.CurrentCulture, stackalloc char[StackallocCharBufferSizeLimit]);
                result.AppendFormatted(span[0]);
                result.AppendFormatted(span[1]);

                for (int i = 2; i < span.Length; i++)
                {
                    result.AppendFormatted(span[i]);
                }

                return result.ToStringAndClear();
            }
            else
            {
                if (!e.TryGetNext(out var first))
                {
                    return ""; // sequence has no value
                }

                if (!e.TryGetNext(out var second))
                {
                    return first!.ToString() ?? ""; // return only first
                }

                var result = new DefaultInterpolatedStringHandler(0, 0, CultureInfo.CurrentCulture, stackalloc char[StackallocCharBufferSizeLimit]);
                result.AppendFormatted(first);
                result.AppendFormatted(second);

                while (e.TryGetNext(out var value))
                {
                    result.AppendFormatted(value);
                }

                return result.ToStringAndClear();
            }
        }
        else
        {
            if (e.TryGetSpan(out var span))
            {
                if (span.Length == 0) return "";
                if (span.Length == 1) return span[0]!.ToString() ?? "";

                var result = new DefaultInterpolatedStringHandler(0, 0, CultureInfo.CurrentCulture, stackalloc char[StackallocCharBufferSizeLimit]);
                result.AppendFormatted(span[0]);
                result.AppendFormatted(separator);
                result.AppendFormatted(span[1]);

                for (int i = 2; i < span.Length; i++)
                {
                    result.AppendFormatted(separator);
                    result.AppendFormatted(span[i]);
                }

                return result.ToStringAndClear();
            }
            else
            {
                if (!e.TryGetNext(out var first))
                {
                    return ""; // sequence has no value
                }

                if (!e.TryGetNext(out var second))
                {
                    return first!.ToString() ?? ""; // return only first
                }

                var result = new DefaultInterpolatedStringHandler(0, 0, CultureInfo.CurrentCulture, stackalloc char[StackallocCharBufferSizeLimit]);
                result.AppendFormatted(first);
                result.AppendFormatted(separator);
                result.AppendFormatted(second);

                while (e.TryGetNext(out var value))
                {
                    result.AppendFormatted(separator);
                    result.AppendFormatted(value);
                }

                return result.ToStringAndClear();
            }
        }
    }

    // TODO: Optimize for string

    public static unsafe string JoinToString(this ValueEnumerable<FromEnumerable<string>, string> source, char separator)
    {
        if (source.Enumerator.TryGetSpan(out var span))
        {
            return JoinToString(span, separator);
        }

        using var enumerator = source.Enumerator.GetSource().GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return ""; // sequence has no value
        }
        var first = enumerator.Current;

        if (!enumerator.MoveNext())
        {
            return first ?? ""; // return only first
        }

        var stringBuilder = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);

        stringBuilder.Append(first);
        do
        {
            stringBuilder.Append(separator);
            stringBuilder.Append(enumerator.Current);
        }
        while (enumerator.MoveNext());

        return stringBuilder.ToStringAndClear();
    }

    static string JoinToString(ReadOnlySpan<string> source, char separator)
    {
        if (source.Length == 0) return "";
        if (source.Length == 1) return source[0];

        // calculate final length
        var finalLength = 0;
        for (int i = 0; i < source.Length; i++)
        {
            finalLength += source[i].Length;
        }
        // add separator length
        finalLength += (1 * (source.Length - 1));

        // internal checks if (c != '\0') so same as FastAllocateString However, this does not apply to legacy mono(Unity).
        var str = new string('\0', finalLength);
        // Span<char> dest = MemoryMarshal.AsRef(ref str.AsSpan());
        // TODO: string as Span<char>
        // TODO: fill values
        // source[0].AsSpan().CopyTo(dest);

        return str;
    }


    // TODO: benchmark hack

    //    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> source, string separator)
    //    {
    //        return JoinToString(source, separator.AsSpan());
    //    }

    //    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> source, char separator)
    //    {
    //#if NET8_0_OR_GREATER
    //        var span = new ReadOnlySpan<char>(ref separator);
    //#else
    //        ReadOnlySpan<char> span = stackalloc char[] { separator };
    //#endif
    //        return JoinToString(source, span);
    //    }

    //    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> source, ReadOnlySpan<char> separator)
    //    {
    //        var list = source.Enumerator.GetSource();
    //        var predicate = source.Enumerator.Predicate;
    //        var selector = source.Enumerator.Selector;

    //        var span = CollectionsMarshal.AsSpan(list);

    //        if (span.Length == 0) return "";

    //        var result = new DefaultInterpolatedStringHandler(0, 0, CultureInfo.CurrentCulture, stackalloc char[StackallocCharBufferSizeLimit]);
    //        var i = 0;
    //        for (; i < span.Length; i++)
    //        {
    //            if (predicate(span[i]))
    //            {
    //                result.AppendFormatted(selector(span[i]));
    //            }
    //        }

    //        for (; i < span.Length; i++)
    //        {
    //            if (predicate(span[i]))
    //            {
    //                result.AppendFormatted(separator);
    //                result.AppendFormatted(selector(span[i]));
    //            }
    //        }

    //        return result.ToStringAndClear();
    //    }
}
