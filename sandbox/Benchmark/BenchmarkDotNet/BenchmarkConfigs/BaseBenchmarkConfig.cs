using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using Perfolizer.Horology;

namespace Benchmark;

/// <summary>
/// Base benchmark config.
/// </summary>
public abstract class BaseBenchmarkConfig : ManualConfig
{
    public BaseBenchmarkConfig()
    {
        WithSummaryStyle(SummaryStyle.Default
                                     .WithMaxParameterColumnWidth(40)); // Default: 20 chars

        WithOrderer(new DefaultOrderer(jobOrderPolicy: JobOrderPolicy.Numeric));

        // Set default options that based on DefaultConfig.
        WithUnionRule(DefaultConfig.Instance.UnionRule);
        WithArtifactsPath(DefaultConfig.Instance.ArtifactsPath);

#if DEBUG
        // Allow benchmarks for debug build.
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
#endif

        // Additional settings used for debugging
        // WithOptions(ConfigOptions.StopOnFirstError);
        // WithOptions(ConfigOptions.KeepBenchmarkFiles);
        // WithOptions(ConfigOptions.GenerateMSBuildBinLog);
    }

    /// <summary>
    /// Use recommended jobsettings of dotnet/performance repository.
    /// https://github.com/dotnet/performance/blob/0396fc87beea5ab8c04a22c1946b3e8901544b22/src/harness/BenchmarkDotNet.Extensions/RecommendedConfig.cs#L35-L40
    /// </summary>
    protected virtual Job GetBaseJobConfig() =>
        Job.Default
           .WithLaunchCount(1)
           .WithWarmupCount(1)
           .WithIterationTime(TimeInterval.FromMilliseconds(250)) // Default: 500ms
           .WithMinIterationCount(15) // Default: 15
           .WithMaxIterationCount(20) // Default: 100
           .WithStrategy(RunStrategy.Throughput) // Explicitly specify RunStrategy (it show `Default` when it's not explicitly specified)
           .DontEnforcePowerPlan();

    /// <summary>
    /// Add configurations.
    /// </summary>
    protected void AddConfigurations()
    {
        AddLoggers();
        AddColumnProviders();
        AddValidators();
        AddFilters();
        AddDiagnosers();
        AddAnalyzers();
        AddEventProcessors();
        AddExporters();
        AddHardwareCounters();
        AddColumnHidingRules();
        AddLogicalGroupRules();
    }

    protected virtual void AddExporters()
    {
        AddExporter(MarkdownExporter.Console); // Use ConsoleMarkdownExporter to disable group higligting with `**`.
    }

    protected virtual void AddLoggers()
    {
        // Set NullLogger to suppress verbose benchmark progress logs. Benchmark progress is reported by custom EventProcessor instead.
        AddLogger(NullLogger.Instance);
    }

    protected virtual void AddAnalyzers()
    {
        AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray());
    }

    protected virtual void AddColumnProviders()
    {
        AddColumnProvider(DefaultColumnProviders.Instance); // Descriptor, Job, Statistics, Params, Metrics
    }

    protected virtual void AddFilters()
    {
        AddFilter(TargetFrameworkFilter.Instance);
    }

    protected virtual void AddDiagnosers()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddDiagnoser(new ExceptionDiagnoser(new ExceptionDiagnoserConfig(displayExceptionsIfZeroValue: false)));
        AddDiagnoser(new ThreadingDiagnoser(new ThreadingDiagnoserConfig(displayCompletedWorkItemCountWhenZero: false, displayLockContentionWhenZero: false)));
    }

    protected virtual void AddValidators()
    {
        AddValidator(DefaultConfig.Instance.GetValidators().ToArray());
    }

    protected virtual void AddEventProcessors()
    {
        // Use custom event processor to report benchmark progress.
        AddEventProcessor(BenchmarkEventProcessor.Instance);
    }

    protected virtual void AddHardwareCounters()
    {
    }

    protected virtual void AddColumnHidingRules()
    {
    }

    protected virtual void AddLogicalGroupRules()
    {
    }
}
