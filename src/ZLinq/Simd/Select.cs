#if NET8_0_OR_GREATER

using System;
using System.Numerics;

namespace ZLinq.Simd;

partial struct Vectorizable<T>
{
    public SelectVectorizable<T, TResult> Select<TResult>(Func<Vector<T>, Vector<TResult>> vectorSelector, Func<T, TResult> selector)
    {
        return new(source, vectorSelector, selector);
    }
}

// TODO: ToArray, CopyTo

public readonly ref struct SelectVectorizable<T, TResult>(ReadOnlySpan<T> source, Func<Vector<T>, Vector<TResult>> vectorSelector, Func<T, TResult> selector)
    where T : unmanaged
{
    readonly ReadOnlySpan<T> source = source;

    public TResult[] ToArray()
    {
        var src = source;

        var result = GC.AllocateUninitializedArray<TResult>(src.Length);
        var destination = result.AsSpan();

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && src.Length >= Vector<T>.Count)
        {
            var vectors = MemoryMarshal.Cast<T, Vector<T>>(src);
            foreach (var item in vectors)
            {
                vectorSelector(item).CopyTo(destination);
                destination = destination.Slice(Vector<T>.Count);
            }

            src = src.Slice(vectors.Length * Vector<T>.Count);
        }

        for (int i = 0; i < src.Length; i++)
        {
            destination[i] = selector(src[i]);
        }

        return result;
    }
}

#endif
