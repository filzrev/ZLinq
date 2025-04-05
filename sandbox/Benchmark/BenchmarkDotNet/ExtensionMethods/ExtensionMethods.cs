using BenchmarkDotNet.Engines;
using ZLinq;

namespace Benchmark;

internal static class ExtensionMethods
{
    /// <summary>
    /// Executes and consumes given <see cref="ValueEnumerable{TEnumerator, T}"/>.
    /// </summary>
    public static void Consume<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable, Consumer consumer)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        using var e = enumerable.Enumerator;
        while (e.TryGetNext(out var item))
        {
            consumer.Consume(in item);
        }
    }

    /// <summary>
    /// Gets display name of specified type.
    /// </summary>
    public static string GetDisplayName(this Type typeInfo)
    {
        if (!typeInfo.IsGenericType)
            return typeInfo.Name;

        // Resolve generics name
        string name = typeInfo.Name.Substring(0, typeInfo.Name.IndexOf('`'));
        string args = string.Join(", ", typeInfo.GetGenericArguments().Select(GetDisplayName));
        return $"{name}<{args}>";
    }
}
