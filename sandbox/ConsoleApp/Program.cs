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

[assembly: ZLinqDropIn("ZLinq", DropInGenerateTypes.Everything)]


Span<int> span = stackalloc int[10];
var list = new List<int>();
var array = new[] { 1, 2, 3, 4, 5 };
IEnumerable<int> ie = array;
// var foo = span.Contains(3);

// span.CopyTo

//list.CopyTo(

// list.Contains(10);

// MemoryExtensions.Contains, CopyTo, Reverse, SequenceEqual
// MemoryExtensions.

// MemoryExtensions.
// span.CopyTo

var xs = new[] { 1, 2, 3 };

xs.Contains(10);


list.CopyTo(span);


// ie.CopyTo

//Console.WriteLine(foo);
