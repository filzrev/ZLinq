﻿namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Union<TEnumerable, TSource> Union<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second);

        public static Union2<TEnumerable, TSource> Union<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, comparer);

    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Union<TEnumerable, TSource>(TEnumerable source, IEnumerable<TSource> second)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<Union<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
            // return source.TryGetNonEnumeratedCount(count);
            // count = 0;
            // return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryGetNext(out TSource current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Union2<TEnumerable, TSource>(TEnumerable source, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<Union2<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
            // return source.TryGetNonEnumeratedCount(count);
            // count = 0;
            // return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryGetNext(out TSource current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

}
