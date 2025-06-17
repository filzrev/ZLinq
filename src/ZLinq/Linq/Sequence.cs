#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromSequence<T>, T> Sequence<T>(T start, T endInclusive, T step)
            where T : INumber<T>
        {
            if (start is null) Throws.Null(nameof(start));
            if (endInclusive is null) Throws.Null(nameof(endInclusive));
            if (step is null) Throws.Null(nameof(step));

            if (step > T.Zero)
            {
                // TODO: impl increment pattern
                // Enumerable.Sequence has known primitive + 1 step has use Range(FillIncremental) optimization
                // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Sequence.cs
            }
            else if (step < T.Zero)
            {
                // TODO: impl decrement pattern
            }
            else
            {
                // TODO: impl repeat one pattern
            }

            throw new NotImplementedException();
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromSequence<T>(T start, T endInclusive, T step, bool isIncrement) : IValueEnumerator<T>
    {
        public bool TryGetNonEnumeratedCount(out int count)
        {
            // TODO: can calculate count?
            throw new NotImplementedException();
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
            // TODO: we can use fill-incremental?
            throw new NotImplementedException();
        }

        public bool TryGetNext(out T current)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}

#endif
