#if NET8_0_OR_GREATER

using System;
using System.Numerics;

namespace ZLinq.Simd;

partial struct Vectorizable<T>
{
    // TODO: Count(predicate)
    //public int Count(Func<Vector<T>, Vector<T>> vectorMask, Func<T, bool> predicate)
    //{
    //    var src = source;
    //    var count = 0;

    //    if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && src.Length >= Vector<T>.Count)
    //    {
    //        var vectors = MemoryMarshal.Cast<T, Vector<T>>(src);
    //        foreach (var item in vectors)
    //        {
    //            var mask = vectorMask(item); // match???

    //            var ones = Vector.ConditionalSelect(mask, Vector<T>.One, Vector<T>.Zero);
    //            var foo = Vector.Sum(ones); // TODO: to int
    //        }

    //        src = src.Slice(vectors.Length * Vector<T>.Count);
    //    }

    //    foreach (T item in src)
    //    {
    //        if (predicate(item))
    //        {
    //            count++;
    //        }
    //    }

    //    return count;
    //}
}

#endif
