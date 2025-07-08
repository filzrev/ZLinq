using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

// Geneated from FileGen.Commands.cs

// support for all platforms
// byte/sbyte/ushort/char/short/uint/int/ulong/long/nuint/nint (+SIMD optimize)
// float/double/decimal/DateTime/DateTimeOffset

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromByteSequence, Byte> Sequence(Byte start, Byte endInclusive, Byte step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromByteSequence(Byte currentValue, Byte endInclusive, Byte step, bool isIncrement) : IValueEnumerator<Byte>
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

        public bool TryGetSpan(out ReadOnlySpan<Byte> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Byte> destination, Index offset)
        {
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (byte)(currentValue + fillStart));
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out Byte current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((byte)(currentValue + step));

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
            if (step == 1 && endInclusive - currentValue + 1 <= Byte.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Byte> span, Byte start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Byte>.IsSupported
#if NET8_0
            && Vector<Byte>.Count <= 16
#endif
                && span.Length >= Vector<Byte>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Byte>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Byte>((ReadOnlySpan<Byte>)new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<Byte>.Count = 8
                var data = indices + new Vector<Byte>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Byte>((byte)Vector<Byte>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Byte>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Byte>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromSByteSequence, SByte> Sequence(SByte start, SByte endInclusive, SByte step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromSByteSequence(SByte currentValue, SByte endInclusive, SByte step, bool isIncrement) : IValueEnumerator<SByte>
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

        public bool TryGetSpan(out ReadOnlySpan<SByte> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<SByte> destination, Index offset)
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

        public bool TryGetNext(out SByte current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= SByte.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<SByte> span, SByte start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<SByte>.IsSupported
#if NET8_0
            && Vector<SByte>.Count <= 16
#endif
                && span.Length >= Vector<SByte>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<SByte>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<SByte>((ReadOnlySpan<SByte>)new SByte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<SByte>.Count = 8
                var data = indices + new Vector<SByte>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<SByte>(Vector<SByte>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<SByte>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<SByte>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromUInt16Sequence, UInt16> Sequence(UInt16 start, UInt16 endInclusive, UInt16 step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromUInt16Sequence(UInt16 currentValue, UInt16 endInclusive, UInt16 step, bool isIncrement) : IValueEnumerator<UInt16>
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

        public bool TryGetSpan(out ReadOnlySpan<UInt16> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<UInt16> destination, Index offset)
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

        public bool TryGetNext(out UInt16 current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= UInt16.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<UInt16> span, UInt16 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<UInt16>.IsSupported
#if NET8_0
            && Vector<UInt16>.Count <= 16
#endif
                && span.Length >= Vector<UInt16>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<UInt16>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<UInt16>((ReadOnlySpan<UInt16>)new UInt16[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<UInt16>.Count = 8
                var data = indices + new Vector<UInt16>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<UInt16>(Vector<UInt16>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<UInt16>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<UInt16>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromInt16Sequence, Int16> Sequence(Int16 start, Int16 endInclusive, Int16 step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInt16Sequence(Int16 currentValue, Int16 endInclusive, Int16 step, bool isIncrement) : IValueEnumerator<Int16>
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

        public bool TryGetSpan(out ReadOnlySpan<Int16> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Int16> destination, Index offset)
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

        public bool TryGetNext(out Int16 current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= Int16.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Int16> span, Int16 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Int16>.IsSupported
#if NET8_0
            && Vector<Int16>.Count <= 16
#endif
                && span.Length >= Vector<Int16>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Int16>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Int16>((ReadOnlySpan<Int16>)new Int16[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<Int16>.Count = 8
                var data = indices + new Vector<Int16>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Int16>(Vector<Int16>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Int16>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Int16>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromUInt32Sequence, UInt32> Sequence(UInt32 start, UInt32 endInclusive, UInt32 step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromUInt32Sequence(UInt32 currentValue, UInt32 endInclusive, UInt32 step, bool isIncrement) : IValueEnumerator<UInt32>
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

        public bool TryGetSpan(out ReadOnlySpan<UInt32> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<UInt32> destination, Index offset)
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

        public bool TryGetNext(out UInt32 current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= UInt32.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<UInt32> span, UInt32 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<UInt32>.IsSupported
#if NET8_0
            && Vector<UInt32>.Count <= 16
#endif
                && span.Length >= Vector<UInt32>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<UInt32>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<UInt32>((ReadOnlySpan<UInt32>)new UInt32[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<UInt32>.Count = 8
                var data = indices + new Vector<UInt32>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<UInt32>(Vector<UInt32>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<UInt32>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<UInt32>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromInt32Sequence, Int32> Sequence(Int32 start, Int32 endInclusive, Int32 step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInt32Sequence(Int32 currentValue, Int32 endInclusive, Int32 step, bool isIncrement) : IValueEnumerator<Int32>
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

        public bool TryGetSpan(out ReadOnlySpan<Int32> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Int32> destination, Index offset)
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

        public bool TryGetNext(out Int32 current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= Int32.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Int32> span, Int32 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Int32>.IsSupported
#if NET8_0
            && Vector<Int32>.Count <= 16
#endif
                && span.Length >= Vector<Int32>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Int32>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Int32>((ReadOnlySpan<Int32>)new Int32[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<Int32>.Count = 8
                var data = indices + new Vector<Int32>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Int32>(Vector<Int32>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Int32>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Int32>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromUInt64Sequence, UInt64> Sequence(UInt64 start, UInt64 endInclusive, UInt64 step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromUInt64Sequence(UInt64 currentValue, UInt64 endInclusive, UInt64 step, bool isIncrement) : IValueEnumerator<UInt64>
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

        public bool TryGetSpan(out ReadOnlySpan<UInt64> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<UInt64> destination, Index offset)
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

        public bool TryGetNext(out UInt64 current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= UInt64.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<UInt64> span, UInt64 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<UInt64>.IsSupported
#if NET8_0
            && Vector<UInt64>.Count <= 16
#endif
                && span.Length >= Vector<UInt64>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<UInt64>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<UInt64>((ReadOnlySpan<UInt64>)new UInt64[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<UInt64>.Count = 8
                var data = indices + new Vector<UInt64>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<UInt64>(Vector<UInt64>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<UInt64>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<UInt64>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromInt64Sequence, Int64> Sequence(Int64 start, Int64 endInclusive, Int64 step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInt64Sequence(Int64 currentValue, Int64 endInclusive, Int64 step, bool isIncrement) : IValueEnumerator<Int64>
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

        public bool TryGetSpan(out ReadOnlySpan<Int64> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Int64> destination, Index offset)
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

        public bool TryGetNext(out Int64 current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= Int64.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Int64> span, Int64 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Int64>.IsSupported
#if NET8_0
            && Vector<Int64>.Count <= 16
#endif
                && span.Length >= Vector<Int64>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Int64>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Int64>((ReadOnlySpan<Int64>)new Int64[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<Int64>.Count = 8
                var data = indices + new Vector<Int64>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Int64>(Vector<Int64>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Int64>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Int64>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromCharSequence, Char> Sequence(Char start, Char endInclusive, Char step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromCharSequence(Char currentValue, Char endInclusive, Char step, bool isIncrement) : IValueEnumerator<Char>
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

        public bool TryGetSpan(out ReadOnlySpan<Char> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Char> destination, Index offset)
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

        public bool TryGetNext(out Char current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= Char.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Char> span, Char start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Char>.IsSupported
#if NET8_0
            && Vector<Char>.Count <= 16
#endif
                && span.Length >= Vector<Char>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Char>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Char>((ReadOnlySpan<Char>)new Char[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<Char>.Count = 8
                var data = indices + new Vector<Char>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Char>(Vector<Char>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Char>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Char>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromUIntPtrSequence, UIntPtr> Sequence(UIntPtr start, UIntPtr endInclusive, UIntPtr step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromUIntPtrSequence(UIntPtr currentValue, UIntPtr endInclusive, UIntPtr step, bool isIncrement) : IValueEnumerator<UIntPtr>
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

        public bool TryGetSpan(out ReadOnlySpan<UIntPtr> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<UIntPtr> destination, Index offset)
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

        public bool TryGetNext(out UIntPtr current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= UIntPtr.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<UIntPtr> span, UIntPtr start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<UIntPtr>.IsSupported
#if NET8_0
            && Vector<UIntPtr>.Count <= 16
#endif
                && span.Length >= Vector<UIntPtr>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<UIntPtr>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<UIntPtr>((ReadOnlySpan<UIntPtr>)new UIntPtr[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<UIntPtr>.Count = 8
                var data = indices + new Vector<UIntPtr>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<UIntPtr>(Vector<UIntPtr>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<UIntPtr>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<UIntPtr>.Count);   // move pointer++

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
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromIntPtrSequence, IntPtr> Sequence(IntPtr start, IntPtr endInclusive, IntPtr step)
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
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromIntPtrSequence(IntPtr currentValue, IntPtr endInclusive, IntPtr step, bool isIncrement) : IValueEnumerator<IntPtr>
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

        public bool TryGetSpan(out ReadOnlySpan<IntPtr> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<IntPtr> destination, Index offset)
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

        public bool TryGetNext(out IntPtr current)
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
            if (step == 1 && endInclusive - currentValue + 1 <= IntPtr.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<IntPtr> span, IntPtr start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<IntPtr>.IsSupported
#if NET8_0
            && Vector<IntPtr>.Count <= 16
#endif
                && span.Length >= Vector<IntPtr>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<IntPtr>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<IntPtr>((ReadOnlySpan<IntPtr>)new IntPtr[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<IntPtr>.Count = 8
                var data = indices + new Vector<IntPtr>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<IntPtr>(Vector<IntPtr>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<IntPtr>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<IntPtr>.Count);   // move pointer++

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
}
