using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        // support for all platforms
        // byte/sbyte/ushort/char/short/uint/int/ulong/long/nuint/nint (+SIMD optimize)
        // float/double/decimal/DateTime/DateTimeOffset

        public static ValueEnumerable<FromInt32Sequence, int> Sequence(int start, int endInclusive, int step)
        {
            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
            {
                if (endInclusive < start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // increment pattern
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else
            {
                if (endInclusive > start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // decrement pattern
                return new(new(start, endInclusive, step, isIncrement: false));
            }
        }

        // TODO: move
        [StructLayout(LayoutKind.Auto)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct FromInt32Sequence(int currentValue, int endInclusive, int step, bool isIncrement) : IValueEnumerator<int>
        {
            bool calledGetNext;

            public bool TryGetNonEnumeratedCount(out int count)
            {
                if (CanOptimize())
                {
                    count = endInclusive - currentValue + 1; // currentValue is start if not strated yet.
                    return true;
                }
          
                count = 0;
                return false;
            }

            public bool TryGetSpan(out ReadOnlySpan<int> span)
            {
                span = default;
                return false;
            }

            public bool TryCopyTo(scoped Span<int> destination, Index offset)
            {
                if (TryGetNonEnumeratedCount(out var count))
                {
                    if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                    {
                        FillIncremental(destination.Slice(0, fillCount), currentValue + fillStart);
                        return true;
                    }
                }

                return false;
            }

            public bool TryGetNext(out int current)
            {
                if (!calledGetNext)
                {
                    calledGetNext = true;
                    current = currentValue;
                    return true;
                }

                if (isIncrement)
                {
                    var next = currentValue + step;

                    if (next >= endInclusive || next <= currentValue)
                    {
                        if (next == endInclusive && currentValue != next)
                        {
                            current = currentValue = next;
                            return true;
                        }

                        current = default!;
                        return false;
                    }

                    current = currentValue = next;
                    return true;
                }
                else
                {
                    var next = currentValue + step;

                    if (next <= endInclusive || next >= currentValue)
                    {
                        if (next == endInclusive && currentValue != next)
                        {
                            current = currentValue = next;
                            return true;
                        }

                        current = default!;
                        return false;
                    }

                    current = currentValue = next;
                    return true;
                }
            }

            public void Dispose()
            {
            }

            bool CanOptimize()
            {
                if (step == 1 && endInclusive - currentValue + 1 <= int.MaxValue)
                {
                    return true;
                }
                return false;
            }

            static void FillIncremental(Span<int> span, int start)
            {
                ref var current = ref MemoryMarshal.GetReference(span);
                ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<int>.IsSupported
#if NET8_0
                && Vector<int>.Count <= 16
#endif
                && span.Length >= Vector<int>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<int>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
                var indices = new Vector<int>((ReadOnlySpan<int>)new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<int>.Count = 8
                var data = indices + new Vector<int>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<int>(Vector<int>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<int>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<int>.Count);   // move pointer++

                    // available space for vectorized copy
                    // (current <= to) -> !(current > to)
                } while (!Unsafe.IsAddressGreaterThan(ref current, ref to));

                start = data[0]; // next value for fill
            }
#endif

                // fill rest
                while (Unsafe.IsAddressLessThan(ref current, ref end))
                {
                    current = start++; // reuse local variable
                    current = ref Unsafe.Add(ref current, 1);
                }
            }
        }


#if NET8_0_OR_GREATER

        public static ValueEnumerable<FromSequence<T>, T> Sequence<T>(T start, T endInclusive, T step)
            where T : INumber<T>
        {
            if (start is null) Throws.Null(nameof(start));
            if (T.IsNaN(start)) Throws.ArgumentOutOfRange(nameof(start));
            if (endInclusive is null) Throws.Null(nameof(endInclusive));
            if (T.IsNaN(endInclusive)) Throws.ArgumentOutOfRange(nameof(endInclusive));
            if (step is null) Throws.Null(nameof(step));
            if (T.IsNaN(step)) Throws.ArgumentOutOfRange(nameof(step));

            if (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (T.IsPositive(step))
            {
                // Enumerable.Sequence has known primitive + 1 step has use Range(FillIncremental) optimization but currently we don't do it.
                // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Sequence.cs

                if (endInclusive < start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // increment pattern
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else
            {
                if (endInclusive > start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // decrement pattern
                return new(new(start, endInclusive, step, isIncrement: false));
            }
        }

#endif
    }
}

namespace ZLinq.Linq
{
#if NET8_0_OR_GREATER

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromSequence<T>(T currentValue, T endInclusive, T step, bool isIncrement) : IValueEnumerator<T>
        where T : INumber<T>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = currentValue + step;

                if (next >= endInclusive || next <= currentValue)
                {
                    if (next == endInclusive && currentValue != next)
                    {
                        current = currentValue = next;
                        return true;
                    }

                    current = default!;
                    return false;
                }

                current = currentValue = next;
                return true;
            }
            else
            {
                var next = currentValue + step;

                if (next <= endInclusive || next >= currentValue)
                {
                    if (next == endInclusive && currentValue != next)
                    {
                        current = currentValue = next;
                        return true;
                    }

                    current = default!;
                    return false;
                }

                current = currentValue = next;
                return true;
            }
        }

        public void Dispose()
        {
        }
    }

#endif
}
