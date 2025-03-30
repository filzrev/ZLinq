namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Skip<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, count));
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
    struct Skip<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int skipCount = Math.Max(0, count); // ensure non-negative
        int skipped;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Max(0, count - skipCount); // subtract skip count, ensure non-negative
                return true;
            }

            count = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var actualSkipCount = Math.Min(count, skipCount);

                var remainingCount = count - actualSkipCount;

                if (remainingCount <= 0)
                {
                    return false;
                }

                var offsetInSkipped = offset.GetOffset(remainingCount);

                if (offsetInSkipped < 0 || offsetInSkipped >= remainingCount)
                {
                    return false;
                }

                var sourceOffset = actualSkipCount + offsetInSkipped;

                var elementsAvailable = remainingCount - offsetInSkipped;

                var elementsToCopy = Math.Min(elementsAvailable, destination.Length);

                if (elementsToCopy <= 0)
                {
                    return false;
                }

                return source.TryCopyTo(destination.Slice(0, elementsToCopy), sourceOffset);
            }

            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                if (span.Length <= skipCount)
                {
                    span = default;
                    return true;
                }
                span = span.Slice(skipCount);
                return true;
            }

            span = default;
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            // Skip elements if not already skipped
            while (skipped < skipCount)
            {
                if (!source.TryGetNext(out var _))
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }
                skipped++;
            }

            // Return elements after skipping
            if (source.TryGetNext(out current))
            {
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
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
    struct SkipTake<TEnumerator, TSource>(TEnumerator source, Int32 skipCount, Int32 takeCount)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        internal readonly int skipCount = Math.Max(0, skipCount); // ensure non-negative
        internal readonly int takeCount = Math.Max(0, takeCount); // ensure non-negative
        int skipped;
        int taken;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                // Calculate elements after skipping
                count = Math.Max(0, count - skipCount);
                // Apply take limit
                count = Math.Min(count, takeCount);
                return true;
            }

            count = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var sourceCount))
            {
                // Determine actual number of elements after skipping
                var actualSkipCount = Math.Min(sourceCount, skipCount);
                var availableAfterSkip = sourceCount - actualSkipCount;

                // Apply take limit
                var actualCount = Math.Min(availableAfterSkip, takeCount);

                if (actualCount <= 0)
                {
                    return false;
                }

                // Calculate offset within the resulting sequence
                var offsetInResult = offset.GetOffset(actualCount);

                if (offsetInResult < 0 || offsetInResult >= actualCount)
                {
                    return false;
                }

                // Calculate offset in source sequence (skip + offset)
                var sourceOffset = actualSkipCount + offsetInResult;

                // Calculate elements available after offset
                var elementsAvailable = actualCount - offsetInResult;

                // Calculate elements to copy
                var elementsToCopy = Math.Min(elementsAvailable, destination.Length);

                if (elementsToCopy <= 0)
                {
                    return false;
                }

                return source.TryCopyTo(destination.Slice(0, elementsToCopy), sourceOffset);
            }

            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                // Skip elements
                if (span.Length <= skipCount)
                {
                    span = default;
                    return true;
                }

                span = span.Slice(skipCount);

                // Take elements
                if (span.Length > takeCount)
                {
                    span = span.Slice(0, takeCount);
                }

                return true;
            }

            span = default;
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            // Skip elements if not already skipped
            while (skipped < skipCount)
            {
                if (!source.TryGetNext(out var _))
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }
                skipped++;
            }

            // Check if we've reached the take limit
            if (taken >= takeCount)
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            // Return elements after skipping and before take limit
            if (source.TryGetNext(out current))
            {
                taken++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}
