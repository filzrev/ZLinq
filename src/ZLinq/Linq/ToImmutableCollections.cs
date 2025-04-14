#if NET8_0_OR_GREATER

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ZLinq;

partial class ValueEnumerableExtensions
{
    // ImmutableArray
    // ImmutableList
    // ImmutableDictionary
    // ImmutableSortedDictionary
    // ImmutableHashSet
    // ImmutableSortedSet

    public static ImmutableArray<TSource> ToImmutableArray<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            return ImmutableArray.Create(span);
        }
        else
        {
            var builder = e.TryGetNonEnumeratedCount(out var count)
                ? ImmutableArray.CreateBuilder<TSource>(count)
                : ImmutableArray.CreateBuilder<TSource>();

            while (e.TryGetNext(out var current))
            {
                builder.Add(current);
            }

            return builder.ToImmutable();
        }
    }

    public static ImmutableList<TSource> ToImmutableList<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            return ImmutableList.Create(span);
        }
        else
        {
            var builder = ImmutableList.CreateBuilder<TSource>();
            while (e.TryGetNext(out var current))
            {
                builder.Add(current);
            }
            return builder.ToImmutable();
        }
    }

    // TODO:...

    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionary<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
       where TKey : notnull
    {
        using var e = source.Enumerator;

        var builder = ImmutableDictionary.CreateBuilder<TKey, TSource>(comparer);

        if (e.TryGetSpan(out var span))
        {
            foreach (var current in span)
            {
                builder.Add(keySelector(current), current);
            }
        }
        else
        {
            while (e.TryGetNext(out var current))
            {
                builder.Add(keySelector(current), current);
            }
        }

        return builder.ToImmutable();
    }

}

#endif
