using System.Globalization;

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
}
