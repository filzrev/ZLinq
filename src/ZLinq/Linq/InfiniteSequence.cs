#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        // TODO: impl
        public static ValueEnumerable<FromInfiniteSequence<T>, T> InfiniteSequence<T>(T start, T step)
            where T : IAdditionOperators<T, T, T>
        {
            throw new NotImplementedException();
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInfiniteSequence<T>(T start, T step) : IValueEnumerator<T>
    {
        public bool TryGetNonEnumeratedCount(out int count)
        {
            // TODO:
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
