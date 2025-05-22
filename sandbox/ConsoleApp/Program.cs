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


var xs = new string[] { "aiueo", "kakikukeko", "sasisuseso!" };

var s = xs.AsValueEnumerable().JoinToString("");



Console.WriteLine(s);

