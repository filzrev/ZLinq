﻿//namespace ZLinq
//{
//    partial class ValueEnumerableExtensions
//    {
//        public static CountBy<TEnumerable, TSource, TKey> CountBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
//            where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//            , allows ref struct
//#endif
//            => new(source, keySelector, keyComparer);

//    }
//}

//namespace ZLinq.Linq
//{
//    [StructLayout(LayoutKind.Auto)]
//    [EditorBrowsable(EditorBrowsableState.Never)]
//#if NET9_0_OR_GREATER
//    public ref
//#else
//    public
//#endif
//    struct CountBy<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
//        : IValueEnumerable<KeyValuePair`2>
//        where TEnumerable : struct, IValueEnumerable<TSource>
//#if NET9_0_OR_GREATER
//        , allows ref struct
//#endif
//    {
//        TEnumerable source = source;

//        public ValueEnumerator<CountBy<TEnumerable, TSource, TKey>, KeyValuePair`2> GetEnumerator() => new(this);

//        public bool TryGetNonEnumeratedCount(out int count)
//        {
//            throw new NotImplementedException();
//            // return source.TryGetNonEnumeratedCount(count);
//            // count = 0;
//            // return false;
//        }

//        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair`2> span)
//        {
//            throw new NotImplementedException();
//            // span = default;
//            // return false;
//        }

//        public bool TryGetNext(out KeyValuePair`2 current)
//        {
//            throw new NotImplementedException();
//            // Unsafe.SkipInit(out current);
//            // return false;
//        }

//        public void Dispose()
//        {
//            source.Dispose();
//        }
//    }

//}
