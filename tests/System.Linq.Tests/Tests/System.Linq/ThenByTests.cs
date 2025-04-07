// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class ThenByTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x1 in new int[] { 1, 6, 0, -1, 3 }
                    from x2 in new int[] { 55, 49, 9, -100, 24, 25 }
                    select new { a1 = x1, a2 = x2 };

            Assert.Equal(
                q.OrderByDescending(e => e.a1).ThenBy(f => f.a2),
                q.OrderByDescending(e => e.a1).ThenBy(f => f.a2)
            );
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x1 in new[] { 55, 49, 9, -100, 24, 25, -1, 0 }
                    from x2 in new[] { "!@#$%^", "C", "AAA", "", null, "Calling Twice", "SoS", string.Empty }
                    where !string.IsNullOrEmpty(x2)
                    select new { a1 = x1, a2 = x2 };

            Assert.Equal(
                q.OrderBy(e => e.a2).ThenBy(f => f.a1),
                q.OrderBy(e => e.a2).ThenBy(f => f.a1)
            );
        }


        [Fact]
        public void SourceEmpty()
        {
            int[] source = [];

            Assert.Empty(source.OrderBy(e => e).ThenBy(e => e));
        }

        [Fact]
        public void SecondaryKeysAreUnique()
        {
            var source = new[]
            {
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Tim", City = "Seattle", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Chris", City = "London", Country = "UK" },
                new { Name = "Rob", City = "Kent", Country = "UK" }
            };
            var expected = new[]
            {
                new { Name = "Rob", City = "Kent", Country = "UK" },
                new { Name = "Chris", City = "London", Country = "UK" },
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Tim", City = "Seattle", Country = "USA" }
            };

            Assert.Equal(expected, source.OrderBy(e => e.Country).ThenBy(e => e.City));
        }

        [Fact]
        public void OrderByAndThenByOnSameField()
        {
            var source = new[]
            {
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Prakash", City = "Chennai", Country = "India" },
                new { Name = "Rob", City = "Kent", Country = "UK" }
            };
            var expected = new[]
            {
                new { Name = "Prakash", City = "Chennai", Country = "India" },
                new { Name = "Rob", City = "Kent", Country = "UK" },
                new { Name = "Jim", City = "Minneapolis", Country = "USA" }
            };

            Assert.Equal(expected, source.OrderBy(e => e.Country).ThenBy(e => e.Country, null));
        }

        [Fact]
        public void SecondKeyRepeatAcrossDifferentPrimary()
        {
            var source = new[]
            {
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Tim", City = "Seattle", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Chris", City = "Minneapolis", Country = "USA" },
                new { Name = "Rob", City = "Seattle", Country = "USA" }
            };
            var expected = new[]
            {
                new { Name = "Chris", City = "Minneapolis", Country = "USA" },
                new { Name = "Jim", City = "Minneapolis", Country = "USA" },
                new { Name = "Philip", City = "Orlando", Country = "USA" },
                new { Name = "Rob", City = "Seattle", Country = "USA" },
                new { Name = "Tim", City = "Seattle", Country = "USA" }
            };

            Assert.Equal(expected, source.OrderBy(e => e.Name).ThenBy(e => e.City, null));
        }

        [Fact]
        public void OrderIsStable()
        {
            var source = @"Because I could not stop for Death -
He kindly stopped for me -
The Carriage held but just Ourselves -
And Immortality.".Split([' ', '\n', '\r', '-'], StringSplitOptions.RemoveEmptyEntries);
            var expected = new[]
            {
                "me", "not", "for", "for", "but", "stop", "held", "just", "could", "kindly", "stopped",
                "I", "He", "The", "And", "Death", "Because", "Carriage", "Ourselves", "Immortality."
            };

            Assert.Equal(expected, source.OrderBy(word => char.IsUpper(word[0])).ThenBy(word => word.Length));
        }

        [Fact]
        public void RunOnce()
        {
            var source = @"Because I could not stop for Death -
He kindly stopped for me -
The Carriage held but just Ourselves -
And Immortality.".Split([' ', '\n', '\r', '-'], StringSplitOptions.RemoveEmptyEntries);
            var expected = new[]
            {
                "me", "not", "for", "for", "but", "stop", "held", "just", "could", "kindly", "stopped",
                "I", "He", "The", "And", "Death", "Because", "Carriage", "Ourselves", "Immortality."
            };

            Assert.Equal(expected, source.RunOnce().OrderBy(word => char.IsUpper(word[0])).ThenBy(word => word.Length));
        }

        [Fact]
        public void NullSource()
        {
            IOrderedEnumerable<int> source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.ThenBy(i => i));
        }

        [Fact]
        public void NullKeySelector()
        {
            Func<DateTime, int> keySelector = null;
            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => Enumerable.Empty<DateTime>().OrderBy(e => e).ThenBy(keySelector));
        }

        [Fact]
        public void NullSourceComparer()
        {
            IOrderedEnumerable<int> source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.ThenBy(i => i, null));
        }

        [Fact]
        public void NullKeySelectorComparer()
        {
            Func<DateTime, int> keySelector = null;
            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => Enumerable.Empty<DateTime>().OrderBy(e => e).ThenBy(keySelector, null));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SortsLargeAscendingEnumerableCorrectly(int thenBys)
        {
            const int Items = 100_000;
            IEnumerable<int> expected = NumberRangeGuaranteedNotCollectionType(0, Items);

            IEnumerable<int> unordered = expected.Select(i => i);
            IOrderedEnumerable<int> ordered = unordered.OrderBy(_ => 0);
            switch (thenBys)
            {
                case 1: ordered = ordered.ThenBy(i => i); break;
                case 2: ordered = ordered.ThenBy(i => 0).ThenBy(i => i); break;
                case 3: ordered = ordered.ThenBy(i => 0).ThenBy(i => 0).ThenBy(i => i); break;
            }

            Assert.Equal(expected, ordered);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SortsLargeDescendingEnumerableCorrectly(int thenBys)
        {
            const int Items = 100_000;
            IEnumerable<int> expected = NumberRangeGuaranteedNotCollectionType(0, Items);

            IEnumerable<int> unordered = expected.Select(i => Items - i - 1);
            IOrderedEnumerable<int> ordered = unordered.OrderBy(_ => 0);
            switch (thenBys)
            {
                case 1: ordered = ordered.ThenBy(i => i); break;
                case 2: ordered = ordered.ThenBy(i => 0).ThenBy(i => i); break;
                case 3: ordered = ordered.ThenBy(i => 0).ThenBy(i => 0).ThenBy(i => i); break;
            }

            Assert.Equal(expected, ordered);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SortsLargeRandomizedEnumerableCorrectly(int thenBys)
        {
            const int Items = 100_000;
            var r = new Random(42);

            int[] randomized = Enumerable.Range(0, Items).Select(i => r.Next()).ToArray();

            IOrderedEnumerable<int> orderedEnumerable = randomized.OrderBy(_ => 0);
            switch (thenBys)
            {
                case 1: orderedEnumerable = orderedEnumerable.ThenBy(i => i); break;
                case 2: orderedEnumerable = orderedEnumerable.ThenBy(i => 0).ThenBy(i => i); break;
                case 3: orderedEnumerable = orderedEnumerable.ThenBy(i => 0).ThenBy(i => 0).ThenBy(i => i); break;
            }
            int[] ordered = orderedEnumerable.ToArray();

            Array.Sort(randomized, (a, b) => a - b);
            Assert.Equal(randomized, orderedEnumerable);
        }
    }
}
