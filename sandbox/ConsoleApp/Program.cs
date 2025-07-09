using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;



// var seq1 = ValueEnumerable.Range(DateTime.Now, DateTime.Now.AddDays(7), TimeSpan.FromDays(1), RightBound.Inclusive);

// var v = new Int128();


var seq = ValueEnumerable.Range(start: DateTime.Now, count: 7, step: TimeSpan.FromDays(1));

foreach (var item in seq)
{
    Console.WriteLine(item);
}
