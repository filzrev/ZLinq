#if NET8_0_OR_GREATER
using System.Numerics;

namespace ZLinq.Simd;

/*
 * vectorized loop template.
        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && source.Length >= Vector<T>.Count)
        {
            var vectors = MemoryMarshal.Cast<T, Vector<T>>(source);
            foreach (var item in vectors)
            {
            }

            source = source.Slice(vectors.Length * Vector<T>.Count);
        }

        foreach (T item in source)
        {
        }
*/

public static class VectorizableExtensions
{
    public static Vectorizable<T> AsVectorizable<T>(this T[] source) where T : unmanaged => new(source);
    public static Vectorizable<T> AsVectorizable<T>(this Span<T> source) where T : unmanaged => new(source);
    public static Vectorizable<T> AsVectorizable<T>(this ReadOnlySpan<T> source) where T : unmanaged => new(source);

    public static void VectorizedUpdate<T>(this T[] source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : unmanaged => Update(source, vectorFunc, func);

    public static void VectorizedUpdate<T>(this Span<T> source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : unmanaged => Update(source, vectorFunc, func);

    static void Update<T>(Span<T> source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : unmanaged
    {
        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && source.Length >= Vector<T>.Count)
        {
            var vectors = MemoryMarshal.Cast<T, Vector<T>>(source);
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = vectorFunc(vectors[i]);
            }

            source = source.Slice(vectors.Length * Vector<T>.Count);
        }

        for (int i = 0; i < source.Length; i++)
        {
            source[i] = func(source[i]);
        }
    }
}


// TODO: All, Any, Aggregate, Select, Zip
// TODO: Count(predicate), Where(predicate)
// TOOD: LINQ's Sum, Average, Max, Min, Contains, SequenceEqual, Reverse
// TODO: CopyTo, ToArray

public readonly ref partial struct Vectorizable<T>(ReadOnlySpan<T> source)
    where T : unmanaged
{
    readonly ReadOnlySpan<T> source = source;
}

#endif
