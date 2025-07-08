using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;


var a = ValueEnumerable.Sequence(1, -10, -3);

foreach (var item in a)
{
    Console.WriteLine(item);
}
