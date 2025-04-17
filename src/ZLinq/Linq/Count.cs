using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Int32 Count<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetNonEnumeratedCount(out var count))
                {
                    return count;
                }

                // if Span exists, already TryGetNonEnumeratedCount returns true so no need to get Span.

                count = 0;
                while (enumerator.TryGetNext(out _))
                {
                    checked { count++; }
                }
                return count;
            }
        }

        public static Int32 Count<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);

            using var enumerator = source.Enumerator;

            var count = 0;
            if (enumerator.TryGetSpan(out var span))
            {
                foreach (var current in span)
                {
                    if (predicate(current))
                    {
                        count++; // no need to use checked
                    }
                }

            }
            else
            {
                while (enumerator.TryGetNext(out var current))
                {
                    if (predicate(current))
                    {
                        checked { count++; }
                    }
                }
            }

            return count;
        }
    }
}
