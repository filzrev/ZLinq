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
