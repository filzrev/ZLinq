#nullable enable
#pragma warning disable

using System.Collections;
using Microsoft.DiagnosticsHub;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;
using ZLinq.Linq;
using ZLinq.Simd;
using ZLinq.Traversables;
using System.Security;
using System.Text.RegularExpressions;
using System;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using My.Tako.Yaki;
using System.Diagnostics.CodeAnalysis;
// using MyApp;

//Span<int> xs = stackalloc int[255];

// caseof bool, char, decimal, nint...

// var xs = new[] { 1, 2, 3, 4, 5 };

//byte.MaxValue
// 2147483647

[assembly: ZLinq.ZLinqDropInAttribute("MyApp", ZLinq.DropInGenerateTypes.Everything, DisableEmitSource = false)]


[assembly: ZLinqDropInExternalExtension("ZLinq", "System.Collections.Generic.IReadOnlyCollection`1")]
[assembly: ZLinqDropInExternalExtension("ZLinq", "System.Collections.Generic.IReadOnlyList`1")]



[assembly: ZLinq.ZLinqDropInExternalExtension("My.Tako.Yaki", "My.Tako.Yaki.MyCollection`1", "My.Tako.Yaki.FromMyCollection`1")]

[assembly: ZLinq.ZLinqDropInExternalExtension("My.Tako.Yaki", "My.Tako.Yaki.MyCollection`1+Child")]


// generateNamespace, sourceTypeFullyQualifiedMetadataName, enumeratorTypeFullyQualifiedMetadataName
[assembly: ZLinqDropInExternalExtension("ZLinq", "System.Collections.Immutable.ImmutableArray`1", "ZLinq.Linq.FromImmutableArray`1")]



var dates = ValueEnumerable.Range((ArithmeticDateTime)DateTime.Now, 7, TimeSpan.FromDays(-1)).Select(x => (DateTime)x).ToArray();

var now = DateTime.Now;
var calendarOfThisMonth = ValueEnumerable.Range((ArithmeticDateTime)new DateTime(now.Year, now.Month, 1), DateTime.DaysInMonth(now.Year, now.Month), TimeSpan.FromDays(1));
var calendarOfThisMonth2 = ValueEnumerable.Range(new DateTime(now.Year, now.Month, 1), DateTime.DaysInMonth(now.Year, now.Month), TimeSpan.FromDays(1));


var weeks = ValueEnumerable.Range(DateTime.Now, 7, TimeSpan.FromDays(1));


var foobar = ValueEnumerable.Range(now, now.AddDays(7.0), TimeSpan.FromDays(1), RightBound.Inclusive).Select(x => x).Take(100);



var a = ValueEnumerable.Range(..);
var b = ValueEnumerable.Range(9999..);
var c = ValueEnumerable.Range(5..10); // 5, 6, 7, 8, 9
var z = ValueEnumerable.Range(5..10, RightBound.Inclusive).Select(x => x * 100).Take(10); // 5, 6, 7, 8, 9, 10
var d = ValueEnumerable.Range(3..3);


var hoge = new[] { 1, 10, 100 }.Select(x => x * 99);


var array = ValueEnumerable.Range(1, 1000).Where(x => x % 2 == 1);

//((object)array).GetType();

// IValueEnumerator



var tako = ValueEnumerable.Range(int.MaxValue - 10, 99, (int)3);
foreach (var item in tako)
{
    Console.WriteLine(item);
}


IReadOnlyCollection<int> xs = new[] { 1, 2, 3, 4, 5 };
xs.Select(x => x * x);


var mc = new MyCollection<int>();
mc.Add(1);

var foobarbaz = mc.Select(x => x);


var list = new AddOnlyIntList2();
list.Add(10);
list.Add(20);
list.Add(30);

//foreach (var item in list.Select(x => x * 100))
//{
//    Console.WriteLine(item);
//}
return;


// Enumerable.Range(1,10).to


//var srcFiles = new DirectoryInfo("../../../../../src/ZLinq/Linq/").GetFiles();
//var tstFiles = new DirectoryInfo("../../../../../tests/ZLinq.Tests/Linq/").GetFiles();

//var grouping = srcFiles.AsValueEnumerable()
//    .LeftJoin(tstFiles,
//        x => x.Name,
//        x => x.Name.Replace("Test", ""),
//        (outer, inner) => new { Name = outer.Name, IsTested = inner != null })
//    .GroupBy(x => x.IsTested);

//foreach (var g in grouping)
//{
//    Console.WriteLine(g.Key ? "Tested::::::::::::::::::" : "NotTested::::::::::::::::::");
//    foreach (var item in g)
//    {
//        Console.WriteLine(item.Name);
//    }
//}


return;

//var xssss = new[] { 1, 2, 3 };

//// ValueEnumerable.Range(1, 10).Zip(xssss, xssss);

//var root = new DirectoryInfo("C:\\Program Files (x86)\\Steam");

//var allDlls = root
//    .Descendants()
//    .OfType<FileInfo>()
//    .Where(x => x.Extension == ".dll");

//var grouped = allDlls
//    .GroupBy(x => x.Name)
//    .Select(x => new { FileName = x.Key, Count = x.Count() })
//    .OrderByDescending(x => x.Count);

//foreach (var item in grouped)
//{
//    Console.WriteLine(item);
//}

static IEnumerable<T> Iterate<T>(IEnumerable<T> source)
{
    foreach (var item in source)
    {
        yield return item;
    }
}


//Console.WriteLine(a);
//Console.WriteLine(b);

//Console.WriteLine(a == b);


// System.Text.Json's JsonNode is the target of LINQ to JSON(not JsonDocument/JsonElement).
var json = JsonNode.Parse("""
{
    "nesting": {
      "level1": {
        "description": "First level of nesting",
        "value": 100,
        "level2": {
          "description": "Second level of nesting",
          "flags": [true, false, true],
          "level3": {
            "description": "Third level of nesting",
            "coordinates": {
              "x": 10.5,
              "y": 20.75,
              "z": -5.0
            },
            "level4": {
              "description": "Fourth level of nesting",
              "metadata": {
                "created": "2025-02-15T14:30:00Z",
                "modified": null,
                "version": 2.1
              },
              "level5": {
                "description": "Fifth level of nesting",
                "settings": {
                  "enabled": true,
                  "threshold": 0.85,
                  "options": ["fast", "accurate", "balanced"],
                  "config": {
                    "timeout": 30000,
                    "retries": 3,
                    "deepSetting": {
                      "algorithm": "advanced",
                      "parameters": [1, 1, 2, 3, 5, 8, 13]
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
}
""");

// JsonNode
var origin = json!["nesting"]!["level1"]!["level2"]!;

// JsonNode axis, Children, Descendants, Anestors, BeforeSelf, AfterSelf and ***Self.
foreach (var item in origin.Descendants().Select(x => x.Node).OfType<JsonArray>())
{
    // [true, false, true], ["fast", "accurate", "balanced"], [1, 1, 2, 3, 5, 8, 13]
    Console.WriteLine(item.ToJsonString(JsonSerializerOptions.Web));
}


class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }

    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}

//Console.WriteLine(hoge.Length);





class A;
class B : A;



[ZLinqDropInExtension]
public class MyList<T> : IEnumerable<T>, IValueEnumerable<MyList<T>.ValueEnumerator, T>
{
    public ValueEnumerable<ValueEnumerator, T> AsValueEnumerable()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct ValueEnumerator : IValueEnumerator<T>
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNext(out T current)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            throw new NotImplementedException();
        }
    }

    //public ValueEnumerable<FromValueEnumerable<int, ValueEnumerator>, int> AsValueEnumerable()
    //{
    //    return new(new(new ValueEnumerator()));
    //}

    //public struct ValueEnumerator : IValueEnumerator<int>
    //{
    //    int count;

    //    public bool TryGetNext(out int current)
    //    {
    //        count += 10;
    //        current = count;
    //        return true;
    //    }

    //    public void Dispose()
    //    {
    //    }

    //    public bool TryCopyTo(scoped Span<int> destination, Index offset)
    //    {
    //        return false;
    //    }
    //    public bool TryGetNonEnumeratedCount(out int count)
    //    {
    //        count = 0;
    //        return false;
    //    }

    //    public bool TryGetSpan(out ReadOnlySpan<int> span)
    //    {
    //        span = default;
    //        return false;
    //    }
    //}
}

public readonly struct ArithmeticDateTime(DateTime dateTime)
    : IAdditionOperators<ArithmeticDateTime, TimeSpan, ArithmeticDateTime>,
      ISubtractionOperators<ArithmeticDateTime, TimeSpan, ArithmeticDateTime>
{
    readonly DateTime dateTime = dateTime;

    public static implicit operator ArithmeticDateTime(DateTime dateTime) => new(dateTime);
    public static implicit operator DateTime(ArithmeticDateTime dateTime) => dateTime.dateTime;

    public static ArithmeticDateTime operator +(ArithmeticDateTime left, TimeSpan right) => left.dateTime + right;
    public static ArithmeticDateTime operator -(ArithmeticDateTime left, TimeSpan right) => left.dateTime - right;

    public override string ToString() => dateTime.ToString();
}

public class Takoyaki
{

}


[ZLinqDropInExtension]
public class AddOnlyIntList : IEnumerable<int>
{
    List<int> list = new List<int>();

    public void Add(int x) => list.Add(x);

    public IEnumerator<int> GetEnumerator() => list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
}

[ZLinqDropInExtension]
public class AddOnlyIntList2 : IValueEnumerable<AddOnlyIntList2.Enumerator, int>
{
    List<int> list = new List<int>();

    public void Add(int x) => list.Add(x);

    public ValueEnumerable<Enumerator, int> AsValueEnumerable()
    {
        // you need to write new(new(new())) magic.
        return new(new(new(list)));
    }

    // `public` struct enumerator
    public struct Enumerator(List<int> source) : IValueEnumerator<int>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = CollectionsMarshal.AsSpan(source);
            return true;
        }

        public bool TryCopyTo(scoped Span<int> destination, Index offset)
        {
            // Optional path: if you can not write this, always return false is ok.
            ReadOnlySpan<int> span = CollectionsMarshal.AsSpan(source);
            if (ZLinq.Internal.EnumeratorHelper.TryGetSlice(span, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;

            }
            return false;
        }

        public bool TryGetNext(out int current)
        {
            if (index < source.Count)
            {
                current = source[index];
                index++;
                return true;
            }

            current = default;
            return false;
        }

        public void Dispose() { }
    }
}


//foreach (var item in origin.Descendants().Where(x => x.Name == "hoge"))
//{
//    if (item.Node == null)
//    {
//        Console.WriteLine(item.Name);
//    }
//    else
//    {
//        Console.WriteLine(item.Node.GetPath() + ":" + item.Name);
//    }
//}

// je.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object


namespace ZLinq
{
    public static class AutoInstrumentLinq
    {
        //public static SelectValueEnumerable<FromArray<TSource>, TSource, TResult> Select<TSource, TResult>(this TSource[] source, Func<TSource, TResult> selector)
        //{
        //    return source.AsValueEnumerable().Select(selector);
        //}

        //public static ConcatValueEnumerable2<RangeValueEnumerable, int, ArrayValueEnumerable<int>> Concat2(this RangeValueEnumerable source, ArrayValueEnumerable<int> second)
        //{
        //    return ValueEnumerableExtensions.Concat2<RangeValueEnumerable, int, ArrayValueEnumerable<int>>(source, second);
        //}
    }

    internal static partial class ZLinqTypeInferenceHelper
    {
        //public static TResult Sum<TResult>(this Select<FromArray<int>, int, int?> source, Func<int?, TResult> selector) where TResult : struct, INumber<TResult>
        //{
        //    return ValueEnumerableExtensions.Sum<Select<FromArray<int>, int, int?>, int?, TResult>(source, selector);
        //}
    }

    public static class Test
    {

    }
}

namespace My.Tako.Yaki
{
    public class MyCollection<T> : Collection<T>
        where T : struct
    {

        public class Child : IEnumerable<T>
        {
            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

    public static class MyCollectionExtensions
    {
        public static ValueEnumerable<FromMyCollection<T>, T> AsValueEnumerable<T>(this MyCollection<T> source)
            where T : struct
        {
            return new(new(source));
        }
    }

    public struct FromMyCollection<T> : IValueEnumerator<T>
        where T : struct
    {
        private readonly MyCollection<T> source;
        private int index;
        public FromMyCollection(MyCollection<T> source)
        {
            this.source = source;
            index = -1;
        }
        public bool TryGetNext(out T current)
        {
            if (++index < source.Count)
            {
                current = source[index];
                return true;
            }
            current = default;
            return false;
        }
        public void Dispose() { }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            throw new NotImplementedException();
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
            throw new NotImplementedException();
        }
    }

}
