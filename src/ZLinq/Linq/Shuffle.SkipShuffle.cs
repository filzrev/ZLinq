using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<SkipShuffle<TEnumerator, TSource>, TSource> Take<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator, count, false));
        }

        public static ValueEnumerable<SkipShuffle<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator, count, true));
        }

        // 'Last' variants are syntax sugar. just takes specified count of items randomly from source.

        public static ValueEnumerable<SkipShuffle<TEnumerator, TSource>, TSource> TakeLast<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator, count, false));
        }

        public static ValueEnumerable<SkipShuffle<TEnumerator, TSource>, TSource> SkipLast<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator, count, true));
        }
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
    struct SkipShuffle<TEnumerator, TSource>(Shuffle<TEnumerator, TSource> shuffle, int count, bool skipOrTake)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = shuffle.source;
        RentedArrayBox<TSource>? buffer;
        int index = 0;
        readonly int count = count < 0 ? 0 : count;
        readonly bool skipOrTake = skipOrTake;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (!source.TryGetNonEnumeratedCount(out count))
                return false;

            InitBuffer();

            count = buffer.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            InitBuffer();
            span = buffer.Span;
            return true;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            InitBuffer();

            if (EnumeratorHelper.TryGetSlice<TSource>(buffer.Span, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (buffer == null)
                InitBuffer();

            if (index < buffer.Length)  // use buffer.Length. 'this.count' may be greater.
            {
                current = buffer.UnsafeGetAt(index++);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            buffer?.Dispose();
            source.Dispose();
        }

        [MemberNotNull(nameof(buffer))]
        void InitBuffer()
        {
            if (buffer == null)
            {
                var (array, consumed) = new ValueEnumerable<TEnumerator, TSource>(source).ToArrayPool();
                var count = consumed;

                if (skipOrTake)
                {
                    count -= this.count;

                    if (count < 0)
                        count = 0;
                }
                else
                {
                    if (count > this.count)
                        count = this.count;
                }

                if (count == 0)
                {
                    ArrayPool<TSource>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TSource>());
                    buffer = RentedArrayBox<TSource>.Empty;
                }
                else
                {
                    RandomShared.Shuffle(array.AsSpan(0, consumed), count);  // must slice by array pool consumed length!!
                    buffer = new RentedArrayBox<TSource>(array, count);
                }
            }
        }
    }
}
