// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Original code: https://github.com/dotnet/runtime/blob/v9.0.3/src/libraries/System.Linq/tests/TestExtensions.cs

using System.Collections;

namespace System.Linq;

#nullable disable

public static class TestExtensions
{
    public static IEnumerable<T> RunOnce<T>(this IEnumerable<T> source) =>
        source is null ? null : (source as IList<T>)?.RunOnce() ?? new RunOnceEnumerable<T>(source);

    public static IEnumerable<T> RunOnce<T>(this IList<T> source)
        => source is null ? null : new RunOnceList<T>(source);

    private class RunOnceEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private bool _called;

        public RunOnceEnumerable(IEnumerable<T> source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Assert.False(_called);
            _called = true;
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class RunOnceList<T> : IList<T>
    {
        private readonly IList<T> _source;
        private readonly HashSet<int> _called = [];

        private void AssertAll()
        {
            Assert.Empty(_called);
            _called.Add(-1);
        }

        private void AssertIndex(int index)
        {
            Assert.DoesNotContain(-1, _called);
            Assert.True(_called.Add(index));
        }

        public RunOnceList(IList<T> source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            AssertAll();
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            AssertAll();
            return _source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            AssertAll();
            _source.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int Count => _source.Count;

        public bool IsReadOnly => true;

        public int IndexOf(T item)
        {
            AssertAll();
            return _source.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                AssertIndex(index);
                return _source[index];
            }
            set { throw new NotSupportedException(); }
        }
    }
}
