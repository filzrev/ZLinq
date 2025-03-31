using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using ZLinq;
using ZLinq.Linq;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject Origin;

    void Start()
    {
        for (int i = 0; i < 10000; i++)
        {
            if (i % 100 == 0) Debug.Log(i);
            Enumerable.Range(1, i).AsValueEnumerable().ToList();
            Enumerable.Range(1, i).ToArray().AsValueEnumerable().ToList();
        }

        //Test();
        //Test2();
    }

    IEnumerable<int> Iterate()
    {
        yield return 42;
    }

    public static void Test()
    {
        var tako = ValueEnumerable.Range(1, 10).Select(x => x.ToString());
        var str = string.Join(',', tako.AsEnumerable());
        Debug.Log(str);
    }

    public static void Test2()
    {
        var w = ValueEnumerable.Range(1, 10)
            .Where(x => x % 2 == 0)
            .Take(10)
            .Index()
            .Order()
            .Skip(1)
            .Shuffle()
            .Select(x => x.Item)
            .Prepend(9999)
            .Append(10000)
            .Chunk(99)
            .Distinct();

        foreach (var item in w)
        {
            Debug.Log(item);
        }
    }
}

public static class ZLinqExtensions
{
    public static IEnumerable<T> AsEnumerable<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> valueEnumerable)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        using (var e = valueEnumerable.Enumerator)
        {
            while (e.TryGetNext(out var current))
            {
                yield return current;
            }
        }
    }
}

internal static class CollectionsMarshal
{
    internal static readonly int ListSize;

    static CollectionsMarshal()
    {
        try
        {
            ListSize = typeof(List<>).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Length;
        }
        catch
        {
            ListSize = 3;
        }
    }

    internal static Span<T> AsSpan<T>(this List<T>? list)
    {
        Span<T> span = default;
        if (list is not null)
        {
            if (ListSize == 3)
            {
                var view = Unsafe.As<ListViewA<T>>(list);
                T[] items = view._items;
                span = items.AsSpan(0, list.Count);
            }
            else if (ListSize == 4)
            {
                var view = Unsafe.As<ListViewB<T>>(list);
                T[] items = view._items;
                span = items.AsSpan(0, list.Count);
            }
        }

        return span;
    }

    // This is not polyfill.
    // Unlike the original SetCount, this does not grow if the count is smaller.
    // Therefore, the internal collection size of the List must always be greater than or equal to the count.
    internal static void UnsafeSetCount<T>(this List<T> list, int count)
    {
        if (list is not null)
        {
            if (ListSize == 3)
            {
                var view = Unsafe.As<ListViewA<T>>(list);
                view._size = count;
            }
            else if (ListSize == 4)
            {
                var view = Unsafe.As<ListViewB<T>>(list);

                UnityEngine.Debug.Log("LIST COUNT: " + list.Count);
                UnityEngine.Debug.Log("LISTVIEWB SIZE: " + view._size);
                view._size = count;
            }
        }
    }
}

internal class ListViewA<T>
{
    public T[] _items;
    public int _size;
    public int _version;
}

internal class ListViewB<T>
{
    public T[] _items;
    public int _size;
    public int _version;
    private System.Object _syncRoot; // in .NET Framework
}
