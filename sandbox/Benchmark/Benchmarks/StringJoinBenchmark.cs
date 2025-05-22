using BenchmarkDotNet.Attributes;
using ZLinq;
using ZLinq.Internal;

namespace Benchmark;

public class StringJoinBenchmark
{
    int[] source = default!;

    [GlobalSetup]
    public void Setup()
    {
        source = Enumerable.Range(1, 1000)
                           .Select(_ => Random.Shared.Next())
                           .ToArray();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory(Categories.LINQ)]
    public void SystemLinq()
    {
        _ = string.Join(',', source);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinq()
    {
        _ = source.AsValueEnumerable()
                  .JoinToString(',');
    }
}

public class StringJoinStringBenchmark
{
    string[] source = default!;

    // [Params(5, 10, 100, 1000, 10000)]
    //[Params(5, 10)]
    [Params(5)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        source = Enumerable.Range(1, N)
            .Select(_ => Guid.NewGuid().ToString())
            .ToArray();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public string SystemLinq()
    {
        return string.Join(",", Iterate()); // string and string separator is optimized
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public string ZLinqOld()
    {
        return Iterate().AsValueEnumerable()
                  .JoinToString(",");
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public string ZLinqImprovement()
    {
        return Iterate().AsValueEnumerable()
                  .JoinToString(',');
    }

    [Benchmark]
    public void New()
    {
        Span<char> chars = stackalloc char[256];
        var provider = new SegmentedArrayProvider<char>(chars);
        Span<char> dest = stackalloc char[256];
        provider.Advance(256);
        provider.CopyToAndClear(dest);
    }

    [Benchmark]
    public void New2()
    {
        Span<char> chars = stackalloc char[256];
        Span<char> dest = stackalloc char[256];
        chars.CopyTo(dest);
    }

    IEnumerable<string> Iterate() // to avoid T[] optimization
    {
        foreach (var item in source)
        {
            yield return item;
        }
    }
}
