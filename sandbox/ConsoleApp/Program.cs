using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;


var seq = ValueEnumerable.Sequence(16, 100, 1);

var foo = seq.ToArray();

foreach (var item in foo)
{
    Console.WriteLine(item);
}

Console.WriteLine("---");

foreach (var item in seq)
{
    Console.WriteLine(item);
}
