using BenchmarkDotNet.Loggers;

namespace Benchmark;

/// <summary>
/// Logger to suppress
/// </summary>
public class QuietLogger : ILogger
{
    private readonly ILogger Logger = ConsoleLogger.Default;

    public static readonly QuietLogger Instance = new QuietLogger();

    public bool IsQuietMode { get; internal set; }

    public string Id => Logger.Id;

    public int Priority => Logger.Priority + 1;

    public void Flush() => Logger.Flush();

    public void Write(LogKind logKind, string text)
    {
        if (IsQuietMode)
            return;

        Logger.Write(logKind, text);
    }

    public void WriteLine()
    {
        if (IsQuietMode)
            return;

        Logger.WriteLine();
    }

    public void WriteLine(LogKind logKind, string text)
    {
        if (IsQuietMode)
            return;

        Logger.WriteLine(logKind, text);
    }
}
