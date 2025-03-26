// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Numerics;
using Xunit;

namespace ZLinq.Tests
{
    public class SumTests : EnumerableTests
    {
        #region SourceIsNull - ArgumentNullExceptionThrown

        [Fact]
        public void SumOfInt_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<int> sourceInt = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceInt.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfInt_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<int?> sourceNullableInt = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableInt.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfLong_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<long> sourceLong = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceLong.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfLong_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<long?> sourceNullableLong = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableLong.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfFloat_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<float> sourceFloat = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceFloat.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceFloat.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfFloat_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<float?> sourceNullableFloat = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableFloat.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableFloat.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfDouble_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<double> sourceDouble = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceDouble.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceDouble.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfDouble_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<double?> sourceNullableDouble = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableDouble.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableDouble.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfDecimal_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<decimal> sourceDecimal = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceDecimal.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceDecimal.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfDecimal_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<decimal?> sourceNullableDecimal = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableDecimal.AsValueEnumerable().Sum());
            AssertExtensions.Throws<ArgumentNullException>("source", () => sourceNullableDecimal.AsValueEnumerable().Sum(x => x));
        }

        #endregion

        #region SelectionIsNull - ArgumentNullExceptionThrown

        [Fact]
        public void SumOfInt_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<int> sourceInt = Enumerable.Empty<int>();
            Func<int, int> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceInt.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfNullableOfInt_SelectorIsNull_ArgumentNullExceptionThrown()
        {

            IEnumerable<int?> sourceNullableInt = Enumerable.Empty<int?>();
            Func<int?, int?> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceNullableInt.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfLong_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<long> sourceLong = Enumerable.Empty<long>();
            Func<long, long> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceLong.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfNullableOfLong_SelectorIsNull_ArgumentNullExceptionThrown()
        {

            IEnumerable<long?> sourceNullableLong = Enumerable.Empty<long?>();
            Func<long?, long?> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceNullableLong.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfFloat_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<float> sourceFloat = Enumerable.Empty<float>();
            Func<float, float> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceFloat.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfNullableOfFloat_SelectorIsNull_ArgumentNullExceptionThrown()
        {

            IEnumerable<float?> sourceNullableFloat = Enumerable.Empty<float?>();
            Func<float?, float?> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceNullableFloat.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfDouble_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<double> sourceDouble = Enumerable.Empty<double>();
            Func<double, double> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceDouble.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfNullableOfDouble_SelectorIsNull_ArgumentNullExceptionThrown()
        {

            IEnumerable<double?> sourceNullableDouble = Enumerable.Empty<double?>();
            Func<double?, double?> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceNullableDouble.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfDecimal_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<decimal> sourceDecimal = Enumerable.Empty<decimal>();
            Func<decimal, decimal> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceDecimal.AsValueEnumerable().Sum(selector));
        }

        [Fact]
        public void SumOfNullableOfDecimal_SelectorIsNull_ArgumentNullExceptionThrown()
        {

            IEnumerable<decimal?> sourceNullableDecimal = Enumerable.Empty<decimal?>();
            Func<decimal?, decimal?> selector = null;
            AssertExtensions.Throws<ArgumentNullException>("selector", () => sourceNullableDecimal.AsValueEnumerable().Sum(selector));
        }

        #endregion

        #region SourceIsEmptyCollection - ZeroReturned

        [Fact]
        public void SumOfInt_SourceIsEmptyCollection_ZeroReturned()
        {
            IEnumerable<int> sourceInt = Enumerable.Empty<int>();
            Assert.Equal(0, sourceInt.AsValueEnumerable().Sum());
            Assert.Equal(0, sourceInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfInt_SourceIsEmptyCollection_ZeroReturned()
        {

            IEnumerable<int?> sourceNullableInt = Enumerable.Empty<int?>();
            Assert.Equal(0, sourceNullableInt.AsValueEnumerable().Sum());
            Assert.Equal(0, sourceNullableInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfLong_SourceIsEmptyCollection_ZeroReturned()
        {
            IEnumerable<long> sourceLong = Enumerable.Empty<long>();
            Assert.Equal(0L, sourceLong.AsValueEnumerable().Sum());
            Assert.Equal(0L, sourceLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfLong_SourceIsEmptyCollection_ZeroReturned()
        {

            IEnumerable<long?> sourceNullableLong = Enumerable.Empty<long?>();
            Assert.Equal(0L, sourceNullableLong.AsValueEnumerable().Sum());
            Assert.Equal(0L, sourceNullableLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfFloat_SourceIsEmptyCollection_ZeroReturned()
        {
            IEnumerable<float> sourceFloat = Enumerable.Empty<float>();
            Assert.Equal(0f, sourceFloat.AsValueEnumerable().Sum());
            Assert.Equal(0f, sourceFloat.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfFloat_SourceIsEmptyCollection_ZeroReturned()
        {

            IEnumerable<float?> sourceNullableFloat = Enumerable.Empty<float?>();
            Assert.Equal(0f, sourceNullableFloat.AsValueEnumerable().Sum());
            Assert.Equal(0f, sourceNullableFloat.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfDouble_SourceIsEmptyCollection_ZeroReturned()
        {
            IEnumerable<double> sourceDouble = Enumerable.Empty<double>();
            Assert.Equal(0d, sourceDouble.AsValueEnumerable().Sum());
            Assert.Equal(0d, sourceDouble.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfDouble_SourceIsEmptyCollection_ZeroReturned()
        {

            IEnumerable<double?> sourceNullableDouble = Enumerable.Empty<double?>();
            Assert.Equal(0d, sourceNullableDouble.AsValueEnumerable().Sum());
            Assert.Equal(0d, sourceNullableDouble.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfDecimal_SourceIsEmptyCollection_ZeroReturned()
        {
            IEnumerable<decimal> sourceDecimal = Enumerable.Empty<decimal>();
            Assert.Equal(0m, sourceDecimal.AsValueEnumerable().Sum());
            Assert.Equal(0m, sourceDecimal.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfDecimal_SourceIsEmptyCollection_ZeroReturned()
        {

            IEnumerable<decimal?> sourceNullableDecimal = Enumerable.Empty<decimal?>();
            Assert.Equal(0m, sourceNullableDecimal.AsValueEnumerable().Sum());
            Assert.Equal(0m, sourceNullableDecimal.AsValueEnumerable().Sum(x => x));
        }

        #endregion

        #region SourceIsNotEmpty - ProperSumReturned

        [Fact]
        public void SumOfInt_SourceIsNotEmpty_ProperSumReturned()
        {
            IEnumerable<int> sourceInt = new int[] { 1, -2, 3, -4 };
            Assert.Equal(-2, sourceInt.AsValueEnumerable().Sum());
            Assert.Equal(-2, sourceInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfInt_SourceIsNotEmpty_ProperSumReturned()
        {

            IEnumerable<int?> sourceNullableInt = new int?[] { 1, -2, null, 3, -4, null };
            Assert.Equal(-2, sourceNullableInt.AsValueEnumerable().Sum());
            Assert.Equal(-2, sourceNullableInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfLong_SourceIsNotEmpty_ProperSumReturned()
        {
            IEnumerable<long> sourceLong = new long[] { 1L, -2L, 3L, -4L };
            Assert.Equal(-2L, sourceLong.AsValueEnumerable().Sum());
            Assert.Equal(-2L, sourceLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfLong_SourceIsNotEmpty_ProperSumReturned()
        {

            IEnumerable<long?> sourceNullableLong = new long?[] { 1L, -2L, null, 3L, -4L, null };
            Assert.Equal(-2L, sourceNullableLong.AsValueEnumerable().Sum());
            Assert.Equal(-2L, sourceNullableLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfFloat_SourceIsNotEmpty_ProperSumReturned()
        {
            IEnumerable<float> sourceFloat = new float[] { 1f, 0.5f, -1f, 0.5f };
            Assert.Equal(1f, sourceFloat.AsValueEnumerable().Sum());
            Assert.Equal(1f, sourceFloat.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfFloat_SourceIsNotEmpty_ProperSumReturned()
        {

            IEnumerable<float?> sourceNullableFloat = new float?[] { 1f, 0.5f, null, -1f, 0.5f, null };
            Assert.Equal(1f, sourceNullableFloat.AsValueEnumerable().Sum());
            Assert.Equal(1f, sourceNullableFloat.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfDouble_SourceIsNotEmpty_ProperSumReturned()
        {
            IEnumerable<double> sourceDouble = new double[] { 1d, 0.5d, -1d, 0.5d };
            Assert.Equal(1d, sourceDouble.AsValueEnumerable().Sum());
            Assert.Equal(1d, sourceDouble.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfDouble_SourceIsNotEmpty_ProperSumReturned()
        {

            IEnumerable<double?> sourceNullableDouble = new double?[] { 1d, 0.5d, null, -1d, 0.5d, null };
            Assert.Equal(1d, sourceNullableDouble.AsValueEnumerable().Sum());
            Assert.Equal(1d, sourceNullableDouble.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfDecimal_SourceIsNotEmpty_ProperSumReturned()
        {
            IEnumerable<decimal> sourceDecimal = new decimal[] { 1m, 0.5m, -1m, 0.5m };
            Assert.Equal(1m, sourceDecimal.AsValueEnumerable().Sum());
            Assert.Equal(1m, sourceDecimal.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfDecimal_SourceIsNotEmpty_ProperSumReturned()
        {

            IEnumerable<decimal?> sourceNullableDecimal = new decimal?[] { 1m, 0.5m, null, -1m, 0.5m, null };
            Assert.Equal(1m, sourceNullableDecimal.AsValueEnumerable().Sum());
            Assert.Equal(1m, sourceNullableDecimal.AsValueEnumerable().Sum(x => x));
        }

        #endregion

        #region SourceSumsToOverflow - OverflowExceptionThrown or Infinity returned

        // For testing vectorized overflow, confirms that overflow is detected in multiple vertical lanes
        // and with the overflow occurring at different vector offsets into the list of data. This includes
        // the 5th and 6th vectors in the data to ensure overflow checks after the unrolled loop that processes
        // four vectors at a time.
        public static IEnumerable<object[]> SumOverflowsVerticalVectorLanes()
        {
            for (int element = 0; element < 2; element++)
            {
                for (int verticalOffset = 1; verticalOffset < 6; verticalOffset++)
                {
                    yield return new object[] { element, verticalOffset };
                }
            }
        }

        [Fact]
        public void SumOfInt_SourceSumsToOverflow_OverflowExceptionThrown()
        {
            IEnumerable<int> sourceInt = new int[] { int.MaxValue, 1 };
            Assert.Throws<OverflowException>(() => sourceInt.AsValueEnumerable().Sum());
            Assert.Throws<OverflowException>(() => sourceInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfInt_SourceSumsToOverflowVectorHorizontally_OverflowExceptionThrown()
        {
            int[] sourceInt = new int[Vector<int>.Count * 4];
            Array.Fill(sourceInt, 0);

            for (int i = 0; i < Vector<int>.Count; i++)
            {
                sourceInt[i] = int.MaxValue - 3;
            }
            for (int i = Vector<int>.Count; i < sourceInt.Length; i++)
            {
                sourceInt[i] = 1;
            }

            Assert.Throws<OverflowException>(() => sourceInt.AsValueEnumerable().Sum());
        }

        [Theory]
        [MemberData(nameof(SumOverflowsVerticalVectorLanes))]
        public void SumOfInt_SourceSumsToOverflowVectorVertically_OverflowExceptionThrown(int element, int verticalOffset)
        {
            int[] sourceInt = new int[Vector<int>.Count * 6];
            Array.Fill(sourceInt, 0);

            sourceInt[element] = int.MaxValue;
            sourceInt[element + Vector<int>.Count * verticalOffset] = 1;

            Assert.Throws<OverflowException>(() => sourceInt.AsValueEnumerable().Sum());
        }

        [Fact]
        public void SumOfNullableOfInt_SourceSumsToOverflow_OverflowExceptionThrown()
        {
            IEnumerable<int?> sourceNullableInt = new int?[] { int.MaxValue, null, 1 };
            Assert.Throws<OverflowException>(() => sourceNullableInt.AsValueEnumerable().Sum());
            Assert.Throws<OverflowException>(() => sourceNullableInt.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfLong_SourceSumsToOverflow_OverflowExceptionThrown()
        {
            IEnumerable<long> sourceLong = new long[] { long.MaxValue, 1L };
            Assert.Throws<OverflowException>(() => sourceLong.AsValueEnumerable().Sum());
            Assert.Throws<OverflowException>(() => sourceLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfLong_SourceSumsToOverflowVectorHorizontally_OverflowExceptionThrown()
        {
            long[] sourceLong = new long[Vector<long>.Count * 4];
            Array.Fill(sourceLong, 0);

            for (int i = 0; i < Vector<long>.Count; i++)
            {
                sourceLong[i] = long.MaxValue - 3;
            }
            for (int i = Vector<long>.Count; i < sourceLong.Length; i++)
            {
                sourceLong[i] = 1;
            }

            Assert.Throws<OverflowException>(() => sourceLong.AsValueEnumerable().Sum());
        }

        [Theory]
        [MemberData(nameof(SumOverflowsVerticalVectorLanes))]
        public void SumOfLong_SourceSumsToOverflowVectorVertically_OverflowExceptionThrown(int element, int verticalOffset)
        {
            long[] sourceLong = new long[Vector<long>.Count * 6];
            Array.Fill(sourceLong, 0);

            sourceLong[element] = long.MaxValue;
            sourceLong[element + Vector<long>.Count * verticalOffset] = 1;

            Assert.Throws<OverflowException>(() => sourceLong.AsValueEnumerable().Sum());
        }

        [Fact]
        public void SumOfNullableOfLong_SourceSumsToOverflow_OverflowExceptionThrown()
        {
            IEnumerable<long?> sourceNullableLong = new long?[] { long.MaxValue, null, 1 };
            Assert.Throws<OverflowException>(() => sourceNullableLong.AsValueEnumerable().Sum());
            Assert.Throws<OverflowException>(() => sourceNullableLong.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfFloat_SourceSumsToOverflow_InfinityReturned()
        {
            IEnumerable<float> sourceFloat = new float[] { float.MaxValue, float.MaxValue };
            Assert.True(float.IsPositiveInfinity(sourceFloat.AsValueEnumerable().Sum()));
            Assert.True(float.IsPositiveInfinity(sourceFloat.AsValueEnumerable().Sum(x => x)));
        }

        [Fact]
        public void SumOfNullableOfFloat_SourceSumsToOverflow_InfinityReturned()
        {
            IEnumerable<float?> sourceNullableFloat = new float?[] { float.MaxValue, null, float.MaxValue };
            Assert.True(float.IsPositiveInfinity(sourceNullableFloat.AsValueEnumerable().Sum().Value));
            Assert.True(float.IsPositiveInfinity(sourceNullableFloat.AsValueEnumerable().Sum(x => x).Value));
        }

        [Fact]
        public void SumOfDouble_SourceSumsToOverflow_InfinityReturned()
        {
            IEnumerable<double> sourceDouble = new double[] { double.MaxValue, double.MaxValue };
            Assert.True(double.IsPositiveInfinity(sourceDouble.AsValueEnumerable().Sum()));
            Assert.True(double.IsPositiveInfinity(sourceDouble.AsValueEnumerable().Sum(x => x)));
        }

        [Fact]
        public void SumOfNullableOfDouble_SourceSumsToOverflow_InfinityReturned()
        {
            IEnumerable<double?> sourceNullableDouble = new double?[] { double.MaxValue, null, double.MaxValue };
            Assert.True(double.IsPositiveInfinity(sourceNullableDouble.AsValueEnumerable().Sum().Value));
            Assert.True(double.IsPositiveInfinity(sourceNullableDouble.AsValueEnumerable().Sum(x => x).Value));
        }

        [Fact]
        public void SumOfDecimal_SourceSumsToOverflow_OverflowExceptionThrown()
        {
            IEnumerable<decimal> sourceDecimal = new decimal[] { decimal.MaxValue, 1m };
            Assert.Throws<OverflowException>(() => sourceDecimal.AsValueEnumerable().Sum());
            Assert.Throws<OverflowException>(() => sourceDecimal.AsValueEnumerable().Sum(x => x));
        }

        [Fact]
        public void SumOfNullableOfDecimal_SourceSumsToOverflow_OverflowExceptionThrown()
        {
            IEnumerable<decimal?> sourceNullableDecimal = new decimal?[] { decimal.MaxValue, null, 1m };
            Assert.Throws<OverflowException>(() => sourceNullableDecimal.AsValueEnumerable().Sum());
            Assert.Throws<OverflowException>(() => sourceNullableDecimal.AsValueEnumerable().Sum(x => x));
        }

        #endregion
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new int?[] { 9999, 0, 888, -1, 66, null, -777, 1, 2, -12345 }
                    where x > int.MinValue
                    select x;
            Assert.Equal(q.Sum(), q.Sum());
        }

        [Fact]
        public void SolitaryNullableSingle()
        {
            float?[] source = { 20.51f };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NaNFromSingles()
        {
            float?[] source = { 20.45f, 0f, -10.55f, float.NaN };
            Assert.True(float.IsNaN(source.AsValueEnumerable().Sum().Value));
        }

        [Fact]
        public void NullableSingleAllNull()
        {
            Assert.Equal(0, Enumerable.Repeat(default(float?), 4).AsValueEnumerable().Sum().Value);
        }

        [Fact]
        public void NullableSingleToNegativeInfinity()
        {
            float?[] source = { -float.MaxValue, -float.MaxValue };
            Assert.True(float.IsNegativeInfinity(source.AsValueEnumerable().Sum().Value));
        }

        [Fact]
        public void NullableSingleFromSelector()
        {
            var source = new[]{
                new { name="Tim", num=(float?)9.5f },
                new { name="John", num=default(float?) },
                new { name="Bob", num=(float?)8.5f }
            };
            Assert.Equal(18.0f, source.AsValueEnumerable().Sum(e => e.num).Value);
        }

        [Fact]
        public void SolitaryInt32()
        {
            int[] source = { 20 };
            Assert.Equal(source.FirstOrDefault(), source.Sum());
        }

        [Fact]
        public void OverflowInt32Negative()
        {
            int[] source = { -int.MaxValue, 0, -5, -20 };
            Assert.Throws<OverflowException>(() => source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void Int32FromSelector()
        {
            var source = new[]
            {
                new { name="Tim", num=10 },
                new { name="John", num=50 },
                new { name="Bob", num=-30 }
            };
            Assert.Equal(30, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void SolitaryNullableInt32()
        {
            int?[] source = { -9 };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableInt32AllNull()
        {
            Assert.Equal(0, Enumerable.Repeat(default(int?), 5).AsValueEnumerable().Sum().Value);
        }

        [Fact]
        public void NullableInt32NegativeOverflow()
        {
            int?[] source = { -int.MaxValue, 0, -5, null, null, -20 };
            Assert.Throws<OverflowException>(() => source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableInt32FromSelector()
        {
            var source = new[]
            {
                new { name="Tim", num=(int?)10 },
                new { name="John", num=default(int?) },
                new { name="Bob", num=(int?)-30 }
            };
            Assert.Equal(-20, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void RunOnce()
        {
            var source = new[]
            {
                new { name="Tim", num=(int?)10 },
                new { name="John", num=default(int?) },
                new { name="Bob", num=(int?)-30 }
            };
            Assert.Equal(-20, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void SolitaryInt64()
        {
            long[] source = { int.MaxValue + 20L };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableInt64NegativeOverflow()
        {
            long[] source = { -long.MaxValue, 0, -5, 20, -16 };
            Assert.Throws<OverflowException>(() => source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void Int64FromSelector()
        {
            var source = new[]{
                new { name="Tim", num=10L },
                new { name="John", num=(long)int.MaxValue },
                new { name="Bob", num=40L }
            };

            Assert.Equal(int.MaxValue + 50L, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void SolitaryNullableInt64()
        {
            long?[] source = { -int.MaxValue - 20L };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableInt64AllNull()
        {
            Assert.Equal(0, Enumerable.Repeat(default(long?), 5).AsValueEnumerable().Sum().Value);
        }

        [Fact]
        public void Int64NegativeOverflow()
        {
            long?[] source = { -long.MaxValue, 0, -5, -20, null, null };
            Assert.Throws<OverflowException>(() => source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableInt64FromSelector()
        {
            var source = new[]{
                new { name="Tim", num=(long?)10L },
                new { name="John", num=(long?)int.MaxValue },
                new { name="Bob", num=default(long?) }
            };

            Assert.Equal(int.MaxValue + 10L, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void SolitaryDouble()
        {
            double[] source = { 20.51 };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void DoubleWithNaN()
        {
            double[] source = { 20.45, 0, -10.55, double.NaN };
            Assert.True(double.IsNaN(source.AsValueEnumerable().Sum()));
        }

        [Fact]
        public void DoubleToNegativeInfinity()
        {
            double[] source = { -double.MaxValue, -double.MaxValue };
            Assert.True(double.IsNegativeInfinity(source.AsValueEnumerable().Sum()));
        }

        [Fact]
        public void DoubleFromSelector()
        {
            var source = new[]
            {
                new { name="Tim", num=9.5 },
                new { name="John", num=10.5 },
                new { name="Bob", num=3.5 }
            };

            Assert.Equal(23.5, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void SolitaryNullableDouble()
        {
            double?[] source = { 20.51 };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableDoubleAllNull()
        {
            Assert.Equal(0, Enumerable.Repeat(default(double?), 4).AsValueEnumerable().Sum().Value);
        }

        [Fact]
        public void NullableDoubleToNegativeInfinity()
        {
            double?[] source = { -double.MaxValue, -double.MaxValue };
            Assert.True(double.IsNegativeInfinity(source.AsValueEnumerable().Sum().Value));
        }

        [Fact]
        public void NullableDoubleFromSelector()
        {
            var source = new[]
            {
                new { name="Tim", num=(double?)9.5 },
                new { name="John", num=default(double?) },
                new { name="Bob", num=(double?)8.5 }
            };
            Assert.Equal(18.0, source.AsValueEnumerable().Sum(e => e.num).Value);
        }

        [Fact]
        public void SolitaryDecimal()
        {
            decimal[] source = { 20.51m };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void DecimalNegativeOverflow()
        {
            decimal[] source = { -decimal.MaxValue, -decimal.MaxValue };
            Assert.Throws<OverflowException>(() => source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void DecimalFromSelector()
        {
            var source = new[]
            {
                new {name="Tim", num=20.51m},
                new {name="John", num=10m},
                new {name="Bob", num=2.33m}
            };
            Assert.Equal(32.84m, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void SolitaryNullableDecimal()
        {
            decimal?[] source = { 20.51m };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableDecimalAllNull()
        {
            Assert.Equal(0, Enumerable.Repeat(default(long?), 3).AsValueEnumerable().Sum().Value);
        }

        [Fact]
        public void NullableDecimalNegativeOverflow()
        {
            decimal?[] source = { -decimal.MaxValue, -decimal.MaxValue };
            Assert.Throws<OverflowException>(() => source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void NullableDecimalFromSelector()
        {
            var source = new[]
            {
                new { name="Tim", num=(decimal?)20.51m },
                new { name="John", num=default(decimal?) },
                new { name="Bob", num=(decimal?)2.33m }
            };
            Assert.Equal(22.84m, source.AsValueEnumerable().Sum(e => e.num));
        }

        [Fact]
        public void SolitarySingle()
        {
            float[] source = { 20.51f };
            Assert.Equal(source.FirstOrDefault(), source.AsValueEnumerable().Sum());
        }

        [Fact]
        public void SingleToNegativeInfinity()
        {
            float[] source = { -float.MaxValue, -float.MaxValue };
            Assert.True(float.IsNegativeInfinity(source.AsValueEnumerable().Sum()));
        }

        [Fact]
        public void SingleFromSelector()
        {
            var source = new[]
            {
                new { name="Tim", num=9.5f },
                new { name="John", num=10.5f },
                new { name="Bob", num=3.5f }
            };
            Assert.Equal(23.5f, source.AsValueEnumerable().Sum(e => e.num));
        }

    }
}
