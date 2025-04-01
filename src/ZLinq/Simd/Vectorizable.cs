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
    public static Vectorizable<T> AsVectorizable<T>(this T[] source) where T : struct, INumber<T> => new(source);
    public static Vectorizable<T> AsVectorizable<T>(this Span<T> source) where T : struct, INumber<T> => new(source);
    public static Vectorizable<T> AsVectorizable<T>(this ReadOnlySpan<T> source) where T : struct, INumber<T> => new(source);

    public static void VectorizedUpdate<T>(this T[] source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : struct, INumber<T> => Update(source, vectorFunc, func);

    public static void VectorizedUpdate<T>(this Span<T> source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : struct, INumber<T> => Update(source, vectorFunc, func);

    static void Update<T>(Span<T> source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : struct, INumber<T>
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

public readonly ref partial struct Vectorizable<T>(ReadOnlySpan<T> source)
    where T : struct, INumber<T>
{
    readonly ReadOnlySpan<T> source = source;

    // LINQ Methods

#if NET9_0_OR_GREATER

    public T Sum()
    {
        return source.AsValueEnumerable().Sum();
    }

    public T SumUnchecked()
    {
        return source.AsValueEnumerable().SumUnchecked();
    }

    public double Average()
    {
        return source.AsValueEnumerable().Average();
    }

    public T Max()
    {
        return source.AsValueEnumerable().Max();
    }

    public T Min()
    {
        return source.AsValueEnumerable().Min();
    }

    public bool Contains(T value)
    {
        return source.AsValueEnumerable().Contains(value);
    }

    public bool SequenceEqual(IEnumerable<T> second)
    {
        return source.AsValueEnumerable().SequenceEqual(second);
    }

    public bool SequenceEqual<TEnumerator>(ValueEnumerable<TEnumerator, T> second)
        where TEnumerator : struct, IValueEnumerator<T>, allows ref struct
    {
        return source.AsValueEnumerable().SequenceEqual(second);
    }

#endif
}

#endif
