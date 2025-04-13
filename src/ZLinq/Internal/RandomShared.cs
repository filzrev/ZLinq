using System.Security.Cryptography;

namespace ZLinq.Internal;

internal static class RandomShared
{
    public static void Shuffle<T>(T[] array)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(array.AsSpan());
#else
        Shared.Value.Shuffle(array.AsSpan());
#endif
    }

    public static void Shuffle<T>(Span<T> span)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(span);
#else
        Shared.Value.Shuffle(span);
#endif
    }

    public static void Shuffle<T>(T[] array, int count)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(array.AsSpan(), count);
#else
        Shared.Value.Shuffle(array.AsSpan(), count);
#endif
    }

    public static void Shuffle<T>(Span<T> span, int count)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(span, count);
#else
        Shared.Value.Shuffle(span, count);
#endif
    }

#if !NET8_0_OR_GREATER

    private static ThreadLocal<Random> Shared = new ThreadLocal<Random>(() =>
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var buffer = new byte[sizeof(int)];
            rng.GetBytes(buffer);
            var seed = BitConverter.ToInt32(buffer, 0);
            return new Random(seed);
        }
    });

    static void Shuffle<T>(this Random random, Span<T> values)
    {
        int n = values.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int j = random.Next(i, n);

            if (j != i)
            {
                T temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }
    }

#endif

    static void Shuffle<T>(this Random random, Span<T> values, int count)
    {
        if (count <= 0)
            return;

        int length = values.Length;
        if (count > length)
            count = length;

        int n = length;
        if (n > count)
            n = count;

        if (n == length)
            n--;  // exclude last item

        for (int i = 0; i < n; i++)
        {
            int j = random.Next(i, length);

            if (j != i)
            {
                T temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }
    }
}
