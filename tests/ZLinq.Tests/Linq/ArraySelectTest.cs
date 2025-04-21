using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLinq.Tests.Linq;

public class ArraySelectTest
{
    [Fact]
    public void Select()
    {
        var array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var expected = array.Select(x => x * 10).ToArray();

        var list = new List<int>();
        foreach (var item in array.AsValueEnumerable().Select(x => x * 10))
        {
            list.Add(item);
        }

        list.ToArray().ShouldBe(expected);

        var dest = new int[10];
        array.AsValueEnumerable().Select(x => x * 10).Enumerator.TryCopyTo(dest, 0);
        dest.ToArray().ShouldBe(expected);
    }

    [Fact]
    public void SelectWhere()
    {
        var array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var expected = array.Select(x => x * 10).Where(x => x <= 50).ToArray();

        var list = new List<int>();
        foreach (var item in array.AsValueEnumerable().Select(x => x * 10).Where(x => x <= 50))
        {
            list.Add(item);
        }

        list.ToArray().ShouldBe(expected);
    }

    [Fact]
    public void SelectToArray()
    {
        var array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var expected = array.Select(x => x * 10).ToArray();

        array.AsValueEnumerable().Select(x => x * 10).ToArray().ShouldBe(expected);
    }

    [Fact]
    public void SelectToList()
    {
        var array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var expected = array.Select(x => x * 10).ToList();

        array.AsValueEnumerable().Select(x => x * 10).ToList().ShouldBe(expected);
    }
}
