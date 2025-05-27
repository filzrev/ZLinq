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
using System.Diagnostics.CodeAnalysis;

// [assembly: ZLinqDropIn("ZLinq", DropInGenerateTypes.Everything)]


var xs = Enumerable.Range(1, 100).ToArray();
var a = xs.AsValueEnumerable().ToImmutableArray();

var ys = ValueEnumerable.Range(1, 100);
var b = ys.ToImmutableArray();

// RangeIterator: IList<int>, IReadOnlyList<int>
var zs = Enumerable.Range(1, 100).AsValueEnumerable();
var c = zs.ToImmutableArray();

var zzs = Enumerable.Range(1, 100).Where(_ => true).AsValueEnumerable();
var d = zzs.ToImmutableArray();

Console.WriteLine(a.SequenceEqual(b));
Console.WriteLine(a.SequenceEqual(c));
Console.WriteLine(a.SequenceEqual(d));

