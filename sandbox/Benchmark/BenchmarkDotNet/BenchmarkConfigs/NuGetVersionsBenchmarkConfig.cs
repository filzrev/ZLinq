using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;

namespace Benchmark;

/// <summary>
/// BenchmarkConfig that run benchmarks to compare performance between LocalBuild/NuGet version.
/// </summary>
public class NuGetVersionsBenchmarkConfig : BaseBenchmarkConfig
{
    public NuGetVersionsBenchmarkConfig() : base()
    {
        var baseJobConfig = GetBaseJobConfig()
                                .WithToolchain(Constants.DefaultToolchain)
                                .Freeze();

        // Compare LocalBuild version with latest NuGet package version.
        var targetNuGetVersions = GetTargetZlinqVersions();

        // 1. Add jobs that use ZLinq NuGet package with specified versions
        foreach (var targetVersion in targetNuGetVersions)
        {
            var job = baseJobConfig
               .WithCustomBuildConfiguration(GetCustomBuildConfigurationName(targetVersion))
               .WithArguments(
                [
                   new MsBuildArgument($"/p:{Constants.MSBuildProperties.TargetZLinqVersion}={targetVersion}"),
                   new MsBuildArgument($"/p:{Constants.MSBuildProperties.ZLinqDefineConstants}=\\\"{GetZLinqDefineConstants(targetVersion)}\\\""), // Surround value with `\"` to use list separator char without escaping.
                ])
               .WithId($"v{targetVersion}");

            bool isBaseline = targetVersion == targetNuGetVersions.First();
            if (isBaseline)
                AddJob(job.AsBaseline());
            else
                AddJob(job);
        }

        // 2. Add LocalBuild job
        if (targetNuGetVersions.Count() == 1)
            AddJob(baseJobConfig.WithId("vLocalBuild")); // Add `v` prefix to change display order.

        // Configure additional settings
        AddConfigurations();
    }

    protected override void AddColumnHidingRules()
    {
        HideColumns(Column.Arguments);
        HideColumns(Column.NuGetReferences);
        HideColumns(Column.BuildConfiguration);
    }

    protected override void AddFilters()
    {
        base.AddFilters();
        AddFilter(ZLinqNuGetVersionFilter.Instance);
    }

    protected override void AddLogicalGroupRules()
    {
        AddLogicalGroupRules(
        [
            BenchmarkLogicalGroupRule.ByCategory,
            BenchmarkLogicalGroupRule.ByMethod,
        ]);
    }

    /// <summary>
    /// Gets DefineConstants settings for using target ZLinq version.
    /// </summary>
    private static string GetZLinqDefineConstants(string versionText)
    {
        const string ListSeparator = ";";

        StringBuilder sb = new();

        // Add custom symbol to distinguish NuGet package build.
        sb.Append(Constants.DefineConstants.USE_ZLINQ_NUGET_PACKAGE);

        // Add target package version symbol
        sb.Append($"{ListSeparator}ZLINQ_{versionText.Replace('.', '_')}");

        // v1.2.0 or later supports Immutable/Frozen collection.
        var version = Version.Parse(versionText);
        if (version >= new Version(1, 2, 0))
            sb.Append($"{ListSeparator}{Constants.DefineConstants.ZLINQ_1_2_0_OR_GREATER}");

        // v1.3.1 contains following breaking changes
        // - WhereArray signature is changed to ArrayWhere.
        // - ArraySelect/ListSelect optimization for Select from Array.
        if (version >= new Version(1, 3, 1))
            sb.Append($"{ListSeparator}{Constants.DefineConstants.ZLINQ_1_3_1_OR_GREATER}");

        // v1.4.0 changes the return value of ToArrayPool to PooledArray<T>,
        if (version >= new Version(1, 4, 0))
            sb.Append($"{ListSeparator}{Constants.DefineConstants.ZLINQ_1_4_0_OR_GREATER}");

        return sb.ToString();
    }

    private static string GetCurrentZLinqVersion()
    {
        DirectoryInfo? dirInfo = new DirectoryInfo(AppContext.BaseDirectory);
        while (true)
        {
            if (dirInfo.GetFiles().Any(x => x.Name == "ZLinq.slnx"))
                break;

            dirInfo = dirInfo.Parent;
            if (dirInfo == null)
                throw new FileNotFoundException("ZLinq.slnx is not found.");
        }

        var solutionDir = dirInfo.FullName;
        var resolvedPath = Path.Combine(solutionDir, "src/ZLinq.Unity/Assets/ZLinq.Unity/package.json");
        if (!File.Exists(resolvedPath) && !Directory.Exists(resolvedPath))
            throw new FileNotFoundException($"File is not found. path: {resolvedPath}");

        var json = File.ReadAllText(resolvedPath);
        var latestVersion = JsonNode.Parse(json)!["version"]!.GetValue<string>();
        return latestVersion;
    }

    private static IEnumerable<string> GetTargetZlinqVersions()
    {
        var json = DownloadVersionsJson();

        Console.WriteLine(json);

        var node = JsonNode.Parse(json)!;
        var versions = node["versions"]!.AsArray().GetValues<string>().ToArray();

        // Note: Uncomment following line when comparing all NuGet package versions performance.
        // return versions;

        bool isRunningOnGitHubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
        return isRunningOnGitHubActions
            ? versions.TakeLast(2)  // Compare performance between latest 2 versions.
            : versions.TakeLast(1); // Compare performance between latest/LocalBuild versions.

        // Helper method to download ZLinq package versions.
        static string DownloadVersionsJson()
        {
            // TODO: Replace method to use HttpClient.
            // On some environment .NET based download (.NET/PowerShell) takes about 45 seconds on first access. And currently it can't determine root cause of problems.
            const string url = "https://api.nuget.org/v3-flatcontainer/ZLinq/index.json";
            var startInfo = new ProcessStartInfo
            {
                FileName = "curl",
                Arguments = $"--silent {url}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(startInfo)!;
            return process.StandardOutput.ReadToEnd();
        }
    }

    private static string GetCustomBuildConfigurationName(string targetVersion)
        => $"{Constants.CustomBuildConfigurations.UseZLinqNuGetPackage}_{targetVersion.Replace(".", "_")}";
}
