using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.EventProcessors;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.Results;
using BenchmarkDotNet.Validators;
using System.Diagnostics;
using System.Text;

namespace Benchmark;

/// <summary>
/// Custom EventProcessor to output log grouping message for CI.
/// </summary>
public class LogGroupingEventProcessor : EventProcessor
{
    private readonly ILogger Logger = ConsoleLogger.Default;

    private int totalBenchmarkCount = 0;
    private int completedBenchmarkCount = 0;
    private bool containsMultipleJobIds = false;

    public static readonly EventProcessor Instance = new LogGroupingEventProcessor();

    private LogGroupingEventProcessor() { }

    public override void OnStartValidationStage()
    {
        QuietLogger.Instance.IsQuietMode = true;
    }

    public override void OnStartBuildStage(IReadOnlyList<BuildPartition> partitions)
    {
        Logger.WriteLine($"##[group]Building {partitions.Count} benchmark projects...");
        totalBenchmarkCount = partitions.Sum(x => x.Benchmarks.Length);
        completedBenchmarkCount = 0;
    }

    public override void OnBuildComplete(BuildPartition partition, BuildResult buildResult)
    {
        var benchmarkCase = partition.RepresentativeBenchmarkCase;
        var jobId = benchmarkCase.Job.ResolvedId;

        if (buildResult.IsBuildSuccess)
            return;

        if (!buildResult.IsGenerateSuccess)
            Logger.WriteLine($"[error]Failed to generate benchmark project ({jobId}): {buildResult.ErrorMessage}");
        else
            Logger.WriteLine($"[error]Failed to build benchmark project ({jobId}): {buildResult.ErrorMessage}");
    }

    public override void OnEndBuildStage()
    {
        Logger.WriteLine("##[endgroup]");
    }

    public override void OnStartRunStage()
    {
        Logger.WriteLine("##[group]Start run benchmarks");
    }

    public override void OnStartRunBenchmarksInType(Type type, IReadOnlyList<BenchmarkCase> benchmarks)
    {
        containsMultipleJobIds = benchmarks.DistinctBy(x => x.Job.ResolvedId).Count() > 1;

        Logger.WriteLine($"##[group]Run benchmarks in type: {type.GetDisplayName()}");
    }

    public override void OnStartRunBenchmark(BenchmarkCase benchmarkCase)
    {
        var benchmarkCaseName = GetBenchmarkCaseDisplayName(benchmarkCase);
        var index = (completedBenchmarkCount + 1).ToString();
        var prefixText = $"[{index}/{totalBenchmarkCount}]";
        Logger.WriteLine($"##[group]{prefixText}Run benchmark: {benchmarkCaseName}");
    }

    public override void OnEndRunBenchmark(BenchmarkCase benchmarkCase, BenchmarkReport report)
    {
        ++completedBenchmarkCount;
        Logger.WriteLine("##[endgroup]");
    }

    public override void OnEndRunBenchmarksInType(Type type, Summary summary)
    {
        Logger.WriteLine("##[endgroup]");
    }

    public override void OnEndRunStage()
    {
        Logger.WriteLine("##[endgroup]");
        QuietLogger.Instance.IsQuietMode = false;
    }

    private string GetBenchmarkCaseDisplayName(BenchmarkCase benchmarkCase)
    {
        var sb = new StringBuilder();
        var methodName = benchmarkCase.Descriptor.WorkloadMethod.Name;

        sb.Append(methodName);

        if (benchmarkCase.HasParameters)
        {
            var parametersInfo = benchmarkCase.Parameters.PrintInfo;
            var parametersInfoWithParentheses = $"({parametersInfo})";
            sb.Append($" {parametersInfoWithParentheses}");
        }

        if (containsMultipleJobIds)
            sb.Append($"JobId({benchmarkCase.Job.ResolvedId})");

        return sb.ToString();
    }
}
