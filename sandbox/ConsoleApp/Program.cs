using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;



// var seq1 = ValueEnumerable.Range(DateTime.Now, DateTime.Now.AddDays(7), TimeSpan.FromDays(1), RightBound.Inclusive);

// var v = new Int128();


// 5/13, 5/14, 5/15, 5/16, 5/17, 5/18, 5/19
var daysOfweek = ValueEnumerable.Range(DateTime.Now, 7, TimeSpan.FromDays(1));

// 5/1, 5/2,...,5/31
var now = DateTime.Now;
var calendarOfThisMonth = ValueEnumerable.Range(new DateTime(now.Year, now.Month, 1), DateTime.DaysInMonth(now.Year, now.Month), TimeSpan.FromDays(1));

// 5/1, 5/2,...,
var endlessDate = ValueEnumerable.InfiniteSequence(DateTime.Now, TimeSpan.FromDays(1));
