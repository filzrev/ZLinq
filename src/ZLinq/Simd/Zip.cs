#if NET8_0_OR_GREATER

using System;
using System.Numerics;

namespace ZLinq.Simd;

partial struct Vectorizable<T>
{
    public ZipVectorizable<T, TResult> Zip<TResult>(ReadOnlySpan<T> second, Func<Vector<T>, Vector<T>, Vector<TResult>> vectorSelector, Func<T, T, TResult> selector)
    {
        return new(source, second, vectorSelector, selector);
    }
}

public readonly ref struct ZipVectorizable<T, TResult>(ReadOnlySpan<T> first, ReadOnlySpan<T> second, Func<Vector<T>, Vector<T>, Vector<TResult>> vectorSelector, Func<T, T, TResult> selector)
    where T : struct, INumber<T>
{
    readonly ReadOnlySpan<T> first = first;
    readonly ReadOnlySpan<T> second = second;

    public TResult[] ToArray()
    {
        var result = GC.AllocateUninitializedArray<TResult>(Math.Min(first.Length, second.Length));
        CopyTo(result);
        return result;
    }

    public void CopyTo(Span<TResult> destination)
    {
        var firstSrc = first;
        var secondSrc = second;

        var smallerLength = Math.Min(firstSrc.Length, secondSrc.Length);
        // TODO: throw destination is too small.

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && smallerLength >= Vector<T>.Count)
        {
            var firstVector = MemoryMarshal.Cast<T, Vector<T>>(firstSrc);
            var secondVector = MemoryMarshal.Cast<T, Vector<T>>(secondSrc);

            var smallerVectorLength = Math.Min(firstVector.Length, secondVector.Length);
            for (int i = 0; i < smallerVectorLength; i++)
            {
                var v = vectorSelector(firstVector[i], secondVector[i]);
                v.CopyTo(destination);
                destination = destination.Slice(Vector<T>.Count);
            }

            firstSrc = firstSrc.Slice(smallerVectorLength * Vector<T>.Count);
            secondSrc = secondSrc.Slice(smallerVectorLength * Vector<T>.Count);
        }

        var min = Math.Min(firstSrc.Length, secondSrc.Length);
        for (int i = 0; i < min; i++)
        {
            destination[i] = selector(firstSrc[i], secondSrc[i]);
        }
    }
}

#endif
