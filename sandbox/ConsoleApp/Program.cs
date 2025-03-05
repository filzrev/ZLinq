﻿using ZLinq;


//Span<int> xs = stackalloc int[255];

// caseof bool, char, decimal, nint...

// var xs = new[] { 1, 2, 3, 4, 5 };

Span<int> xs = [1, 2, 3, 4, 5];


var dict = xs.AsValueEnumerable().ToDictionary(x => x);

var takoyakix = xs.AsValueEnumerable().Select(x => x.ToString()).Where(x => x == "foo").Take(100);
// Enumerable.Range(1,10).OrderBy
foreach (var item in takoyakix)
{
    Console.WriteLine(item);
}
xs.AsValueEnumerable().Where(x => x == 0).Select(x => x * x);


xs.AsValueEnumerable().Select(x => (float)x).ToDictionary(x => x);

//Console.WriteLine(hoge.Length);


//var json = JsonNode.Parse("""
//{
//    "nesting": {
//      "level1": {
//        "description": "First level of nesting",
//        "value": 100,
//        "level2": {
//          "description": "Second level of nesting",
//          "flags": [true, false, true],
//          "level3": {
//            "description": "Third level of nesting",
//            "coordinates": {
//              "x": 10.5,
//              "y": 20.75,
//              "z": -5.0
//            },
//            "level4": {
//              "description": "Fourth level of nesting",
//              "metadata": {
//                "created": "2025-02-15T14:30:00Z",
//                "modified": null,
//                "version": 2.1
//              },
//              "level5": {
//                "description": "Fifth level of nesting",
//                "settings": {
//                  "enabled": true,
//                  "threshold": 0.85,
//                  "options": ["fast", "accurate", "balanced"],
//                  "config": {
//                    "timeout": 30000,
//                    "retries": 3,
//                    "deepSetting": {
//                      "algorithm": "advanced",
//                      "parameters": [1, 1, 2, 3, 5, 8, 13]
//                    }
//                  }
//                }
//              }
//            }
//          }
//        }
//      }
//    }
//}
//""");


//var origin = json!["nesting"]!["level1"]!["level2"]!;

//foreach (var item in origin.Descendants().Select(x => x.Node).OfType(default(JsonArray)))
//{
//    Console.WriteLine(item);
//}





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


namespace ZLinq.AutoInstrument
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
}