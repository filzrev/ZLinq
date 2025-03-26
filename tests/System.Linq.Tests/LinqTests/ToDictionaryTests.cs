// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace ZLinq.Tests
{
    public class ToDictionaryTests : EnumerableTests
    {
        [Fact]
        public void ToDictionary_AlwaysCreateACopy()
        {
            Dictionary<int, int> source = new Dictionary<int, int>() { { 1, 1 }, { 2, 2 }, { 3, 3 } };
            Dictionary<int, int> result = source.AsValueEnumerable().ToDictionary(key => key.Key, val => val.Value);

            Xunit.Assert.NotSame(source, result);
            Assert.Equal(source, result);
        }


        private void RunToDictionaryOnAllCollectionTypes<T>(T[] items, Action<Dictionary<T, T>> validation)
        {
            validation(items.AsValueEnumerable().ToDictionary(key => key));
            validation(items.AsValueEnumerable().ToDictionary(key => key, value => value));
            validation(items.AsValueEnumerable().ToDictionary(key => key));
            validation(items.AsValueEnumerable().ToDictionary(key => key, value => value));
            validation(new TestEnumerable<T>(items).AsValueEnumerable().ToDictionary(key => key));
            validation(new TestEnumerable<T>(items).AsValueEnumerable().ToDictionary(key => key, value => value));
            validation(new TestReadOnlyCollection<T>(items).AsValueEnumerable().ToDictionary(key => key));
            validation(new TestReadOnlyCollection<T>(items).AsValueEnumerable().ToDictionary(key => key, value => value));
            validation(new TestCollection<T>(items).AsValueEnumerable().ToDictionary(key => key));
            validation(new TestCollection<T>(items).AsValueEnumerable().ToDictionary(key => key, value => value));
        }

        private void RunToDictionaryFromKvOnAllCollectionTypes<T>(T[] items, Action<Dictionary<T, T>> validation) where T : notnull
        {
            KeyValuePair<T, T>[] kvps = items.Select(item => new KeyValuePair<T, T>(item, item)).ToArray();

            validation(kvps.AsValueEnumerable().ToDictionary(x => x.Key, x => x.Value));
            validation(new List<KeyValuePair<T, T>>(kvps).AsValueEnumerable().ToDictionary());
            validation(new TestEnumerable<KeyValuePair<T, T>>(kvps).AsValueEnumerable().ToDictionary());
            validation(new TestReadOnlyCollection<KeyValuePair<T, T>>(kvps).AsValueEnumerable().ToDictionary());
            validation(new TestCollection<KeyValuePair<T, T>>(kvps).AsValueEnumerable().ToDictionary());

            (T, T)[] vts = items.Select(item => (item, item)).ToArray();

            validation(vts.AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2)); // TODO:
            validation(new List<(T, T)>(vts).AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2));
            validation(new TestEnumerable<(T, T)>(vts).AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2));
            validation(new TestReadOnlyCollection<(T, T)>(vts).AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2));
            validation(new TestCollection<(T, T)>(vts).AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2));
        }

        [Fact]
        public void ToDictionary_WorkWithEmptyCollection()
        {
            RunToDictionaryOnAllCollectionTypes(new int[0],
                resultDictionary =>
                {
                    Assert.NotNull(resultDictionary);
                    Assert.Empty(resultDictionary);
                });
        }

        [Fact]
        public void ToDictionaryFromKv_WorkWithEmptyCollection()
        {
            RunToDictionaryFromKvOnAllCollectionTypes(Array.Empty<int>(),
                resultDictionary =>
                {
                    Assert.NotNull(resultDictionary);
                    Assert.Empty(resultDictionary);
                });
        }

        [Fact]
        public void ToDictionary_ProduceCorrectDictionary()
        {
            int[] sourceArray = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            RunToDictionaryOnAllCollectionTypes(sourceArray,
                resultDictionary =>
                {
                    Assert.Equal(sourceArray.Length, resultDictionary.Count);
                    Assert.Equal(sourceArray, resultDictionary.Keys);
                    Assert.Equal(sourceArray, resultDictionary.Values);
                });

            string[] sourceStringArray = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            RunToDictionaryOnAllCollectionTypes(sourceStringArray,
                resultDictionary =>
                {
                    Assert.Equal(sourceStringArray.Length, resultDictionary.Count);
                    for (int i = 0; i < sourceStringArray.Length; i++)
                        Xunit.Assert.Same(sourceStringArray[i], resultDictionary[sourceStringArray[i]]);
                });
        }

        [Fact]
        public void ToDictionaryFromKv_ProduceCorrectDictionary()
        {
            int[] sourceArray = new[] { 1, 2, 3, 4, 5, 6, 7 };
            RunToDictionaryFromKvOnAllCollectionTypes(sourceArray,
                resultDictionary =>
                {
                    Assert.Equal(sourceArray.Length, resultDictionary.Count);
                    Assert.Equal(sourceArray, resultDictionary.Keys);
                    Assert.Equal(sourceArray, resultDictionary.Values);
                });

            string[] sourceStringArray = new[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            RunToDictionaryFromKvOnAllCollectionTypes(sourceStringArray,
                resultDictionary =>
                {
                    Assert.Equal(sourceStringArray.Length, resultDictionary.Count);
                    foreach (string item in sourceStringArray)
                    {
                        Xunit.Assert.Same(item, resultDictionary[item]);
                    }
                });
        }

        [Fact]
        public void RunOnce()
        {
            Assert.Equal(
                new Dictionary<int, string> { { 1, "0" }, { 2, "1" }, { 3, "2" }, { 4, "3" } },
                Enumerable.Range(0, 4).AsValueEnumerable().ToDictionary(i => i + 1, i => i.ToString()));

            Assert.Equal(
                new Dictionary<int, string> { { 0, "0" }, { 1, "1" }, { 2, "2" }, { 3, "3" } },
                Enumerable.Range(0, 4).AsValueEnumerable().Select(i => new KeyValuePair<int, string>(i, i.ToString())).RunOnce().ToDictionary());

            Assert.Equal(
                new Dictionary<int, string> { { 0, "0" }, { 1, "1" }, { 2, "2" }, { 3, "3" } },
                Enumerable.Range(0, 4).AsValueEnumerable().Select(i => (i, i.ToString())).RunOnce().ToDictionary());
        }

        [Fact]
        public void ToDictionary_PassCustomComparer()
        {
            EqualityComparer<int> comparer = EqualityComparer<int>.Create((x, y) => x == y, x => x);
            TestCollection<int> collection = new TestCollection<int>(new int[] { 1, 2, 3, 4, 5, 6 });

            Dictionary<int, int> result1 = collection.AsValueEnumerable().ToDictionary(key => key, comparer);
            Xunit.Assert.Same(comparer, result1.Comparer);

            Dictionary<int, int> result2 = collection.AsValueEnumerable().ToDictionary(key => key, val => val, comparer);
            Xunit.Assert.Same(comparer, result2.Comparer);

            Dictionary<int, int> result3 = collection.AsValueEnumerable().Select(i => new KeyValuePair<int, int>(i, i)).ToDictionary(comparer);
            Xunit.Assert.Same(comparer, result3.Comparer);

            // TODO: Revert ToDictionary method signature
            Dictionary<int, int> result4 = collection.AsValueEnumerable().Select(i => (i, i)).ToDictionary(x => x.Item1, x => x.Item2, comparer); // TODO:
            Xunit.Assert.Same(comparer, result4.Comparer);
        }

        [Fact]
        public void ToDictionary_UseDefaultComparerOnNull()
        {
            TestCollection<int> collection = new TestCollection<int>(new int[] { 1, 2, 3, 4, 5, 6 });

            Dictionary<int, int> result1 = collection.AsValueEnumerable().ToDictionary(key => key, comparer: null);
            Xunit.Assert.Same(EqualityComparer<int>.Default, result1.Comparer);

            Dictionary<int, int> result2 = collection.AsValueEnumerable().ToDictionary(key => key, val => val, comparer: null);
            Xunit.Assert.Same(EqualityComparer<int>.Default, result2.Comparer);
        }

        [Fact]
        public void ToDictionary_UseDefaultComparer()
        {
            TestCollection<int> collection = new TestCollection<int>(new[] { 1, 2, 3, 4, 5, 6 });

            Dictionary<int, int> result1 = collection.AsValueEnumerable().ToDictionary(key => key);
            Xunit.Assert.Same(EqualityComparer<int>.Default, result1.Comparer);

            Dictionary<int, int> result2 = collection.AsValueEnumerable().ToDictionary(key => key, val => val);
            Xunit.Assert.Same(EqualityComparer<int>.Default, result2.Comparer);

            Dictionary<int, int> result3 = collection.AsValueEnumerable().Select(i => new KeyValuePair<int, int>(i, i)).ToDictionary();
            Xunit.Assert.Same(EqualityComparer<int>.Default, result3.Comparer);

            // TODO: Revert ToDictionary method signature
            Dictionary<int, int> result4 = collection.AsValueEnumerable().Select(i => (i, i)).ToDictionary(x => x.Item1, x => x.Item2); // TODO:
            Xunit.Assert.Same(EqualityComparer<int>.Default, result4.Comparer);
        }

        [Fact]
        public void ToDictionary_KeyValueSelectorsWork()
        {
            TestCollection<int> collection = new TestCollection<int>(new int[] { 1, 2, 3, 4, 5, 6 });

            Dictionary<int, int> result = collection.AsValueEnumerable().ToDictionary(key => key + 10, val => val + 100);

            Assert.Equal(collection.Items.Select(o => o + 10), result.Keys);
            Assert.Equal(collection.Items.Select(o => o + 100), result.Values);
        }


        [Fact]
        public void ToDictionary_ThrowArgumentNullExceptionWhenSourceIsNull()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).AsValueEnumerable().ToDictionary(key => key));
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<KeyValuePair<int, int>>)null).AsValueEnumerable().ToDictionary());
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<(int, int)>)null).AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2)); // TODO:
        }


        [Fact]
        public void ToDictionary_ThrowArgumentNullExceptionWhenKeySelectorIsNull()
        {
            int[] source = new int[0];
            Func<int, int> keySelector = null;
            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => source.AsValueEnumerable().ToDictionary(keySelector));
        }

        [Fact]
        public void ToDictionary_ThrowArgumentNullExceptionWhenValueSelectorIsNull()
        {
            int[] source = new int[0];
            Func<int, int> keySelector = key => key;
            Func<int, int> valueSelector = null;
            AssertExtensions.Throws<ArgumentNullException>("elementSelector", () => source.AsValueEnumerable().ToDictionary(keySelector, valueSelector));
        }

        [Fact]
        public void ToDictionary_ThrowArgumentNullExceptionWhenSourceIsNullElementSelector()
        {
            int[] source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.AsValueEnumerable().ToDictionary(key => key, e => e));
        }


        [Fact]
        public void ToDictionary_ThrowArgumentNullExceptionWhenKeySelectorIsNullElementSelector()
        {
            int[] source = new int[0];
            Func<int, int> keySelector = null;
            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => source.AsValueEnumerable().ToDictionary(keySelector, e => e));
        }

        [Fact]
        public void ToDictionary_KeySelectorThrowException()
        {
            int[] source = new int[] { 1, 2, 3 };
            Func<int, int> keySelector = key =>
            {
                if (key == 1)
                    throw new InvalidOperationException();
                return key;
            };


            Assert.Throws<InvalidOperationException>(() => source.AsValueEnumerable().ToDictionary(keySelector));
        }

        [Fact]
        public void ToDictionary_ThrowWhenKeySelectorReturnNull()
        {
            int[] source = new int[] { 1, 2, 3 };
            Func<int, string> keySelector = key => null;

            AssertExtensions.Throws<ArgumentNullException>("key", () => source.AsValueEnumerable().ToDictionary(keySelector));
        }

        [Fact]
        public void ToDictionary_ThrowWhenKeySelectorReturnSameValueTwice()
        {
            int[] source = new int[] { 1, 2, 3 };
            Func<int, int> keySelector = key => 1;

            AssertExtensions.Throws<ArgumentException>(null, () => source.AsValueEnumerable().ToDictionary(keySelector));
        }

        [Fact]
        public void ToDictionary_ValueSelectorThrowException()
        {
            int[] source = new int[] { 1, 2, 3 };
            Func<int, int> keySelector = key => key;
            Func<int, int> valueSelector = value =>
            {
                if (value == 1)
                    throw new InvalidOperationException();
                return value;
            };

            Assert.Throws<InvalidOperationException>(() => source.AsValueEnumerable().ToDictionary(keySelector, valueSelector));
        }

        [Fact]
        public void ThrowsOnNullKey()
        {
            var source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = "null", Score = 55 }
            };

            source.ToDictionary(e => e.Name); // Doesn't throw;

            source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = default(string), Score = 55 }
            };

            AssertExtensions.Throws<ArgumentNullException>("key", () => source.AsValueEnumerable().ToDictionary(e => e.Name));

            var source2 = new KeyValuePair<string?, int>[]
            {
                new("Chris", 50),
                new("Bob", 95),
                new(default, 55)
            };

            AssertExtensions.Throws<ArgumentNullException>("key", () => source2.AsValueEnumerable().ToDictionary());

            var source3 = new[]
            {
                ("Chris", 50),
                ("Bob", 95),
                (default, 55)
            };

            AssertExtensions.Throws<ArgumentNullException>("key", () => source3.AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2)); // TODO:
        }

        [Fact]
        public void ThrowsOnNullKeyCustomComparer()
        {
            var source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = "null", Score = 55 }
            };

            source.ToDictionary(e => e.Name, new AnagramEqualityComparer()); // Doesn't throw;

            source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = default(string), Score = 55 }
            };

            AssertExtensions.Throws<ArgumentNullException>("key", () => source.AsValueEnumerable().ToDictionary(e => e.Name, new AnagramEqualityComparer()));
        }

        [Fact]
        public void ThrowsOnNullKeyValueSelector()
        {
            var source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = "null", Score = 55 }
            };

            source.ToDictionary(e => e.Name, e => e); // Doesn't throw;

            source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = default(string), Score = 55 }
            };

            AssertExtensions.Throws<ArgumentNullException>("key", () => source.AsValueEnumerable().ToDictionary(e => e.Name, e => e));
        }

        [Fact]
        public void ThrowsOnNullKeyCustomComparerValueSelector()
        {
            var source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = "null", Score = 55 }
            };

            source.AsValueEnumerable().ToDictionary(e => e.Name, e => e, new AnagramEqualityComparer()); // Doesn't throw;

            source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = default(string), Score = 55 }
            };

            AssertExtensions.Throws<ArgumentNullException>("key", () => source.AsValueEnumerable().ToDictionary(e => e.Name, e => e, new AnagramEqualityComparer()));
        }

        [Fact]
        public void ThrowsOnDuplicateKeys()
        {
            var source = new[]
            {
                new { Name = "Chris", Score = 50 },
                new { Name = "Bob", Score = 95 },
                new { Name = "Bob", Score = 55 }
            };

            AssertExtensions.Throws<ArgumentException>(null, () => source.AsValueEnumerable().ToDictionary(e => e.Name, e => e, new AnagramEqualityComparer()));

            var source2 = new KeyValuePair<string, int>[]
            {
                new("Chris", 50),
                new("Bob", 95),
                new("Bob", 55)
            };

            AssertExtensions.Throws<ArgumentException>(null, () => source2.AsValueEnumerable().ToDictionary(new AnagramEqualityComparer()));

            var source3 = new[]
            {
                ("Chris", 50),
                ("Bob", 95),
                ("Bob", 55)
            };

            AssertExtensions.Throws<ArgumentException>(null, () => source3.AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2, new AnagramEqualityComparer())); // TODO:
        }

        private static void AssertMatches<K, E>(IEnumerable<K> keys, IEnumerable<E> values, Dictionary<K, E> dict)
        {
            Assert.NotNull(dict);
            Assert.NotNull(keys);
            Assert.NotNull(values);
            using (var ke = keys.GetEnumerator())
            {
                foreach (var value in values)
                {
                    Assert.True(ke.MoveNext());
                    var key = ke.Current;
                    E dictValue;
                    Assert.True(dict.TryGetValue(key, out dictValue));
                    Assert.Equal(value, dictValue);
                    dict.Remove(key);
                }
                Assert.False(ke.MoveNext());
                Assert.Empty(dict);
            }
        }

        [Fact]
        public void EmptySource()
        {
            int[] elements = new int[] { };
            string[] keys = new string[] { };
            var source = keys.Zip(elements, (k, e) => new { Name = k, Score = e });

            AssertMatches(keys, elements, source.ToDictionary(e => e.Name, e => e.Score, new AnagramEqualityComparer()));
        }

        [Fact]
        public void OneElementNullComparer()
        {
            int[] elements = new int[] { 5 };
            string[] keys = new string[] { "Bob" };
            var source = new[] { new { Name = keys[0], Score = elements[0] } };

            AssertMatches(keys, elements, source.AsValueEnumerable().ToDictionary(e => e.Name, e => e.Score, null));
        }

        [Fact]
        public void SeveralElementsCustomComparerer()
        {
            string[] keys = new string[] { "Bob", "Zen", "Prakash", "Chris", "Sachin" };
            var source = new[]
            {
                new { Name = "Bbo", Score = 95 },
                new { Name = keys[1], Score = 45 },
                new { Name = keys[2], Score = 100 },
                new { Name = keys[3], Score = 90 },
                new { Name = keys[4], Score = 45 }
            };

            AssertMatches(keys, source, source.AsValueEnumerable().ToDictionary(e => e.Name, new AnagramEqualityComparer()));
        }

        [Fact]
        public void NullCoalescedKeySelector()
        {
            string[] elements = new string[] { null };
            string[] keys = new string[] { string.Empty };
            string[] source = new string[] { null };

            AssertMatches(keys, elements, source.AsValueEnumerable().ToDictionary(e => e ?? string.Empty, e => e, EqualityComparer<string>.Default));

        }
    }
}
