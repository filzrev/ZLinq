using System.Buffers;
using System.Numerics;
using System.Security.Cryptography;

namespace ZLinq.Internal;

internal static class RandomShared
{
    public static void Shuffle<T>(Span<T> span)
    {
        Shared.Shuffle(span);
    }

    public static void PartialShuffle<T>(Span<T> span, int count)
    {
        if (count >= span.Length)
        {
            Shared.Shuffle(span);
        }
        else
        {
            Shared.PartialShuffle(span, count);
        }
    }

    internal sealed class Xoshiro256StarStar
    {
        private ulong _s0, _s1, _s2, _s3;

        public Xoshiro256StarStar()
        {
#if NETSTANDARD2_1_OR_GREATER
            var span = (stackalloc ulong[4]);
            do
            {
                RandomNumberGenerator.Fill(MemoryMarshal.AsBytes(span));
                _s0 = span[0];
                _s1 = span[1];
                _s2 = span[2];
                _s3 = span[3];
            } while (_s0 == 0 && _s1 == 0 && _s2 == 0 && _s3 == 0);
#else
            var array = ArrayPool<byte>.Shared.Rent(32);
            using var rng = RandomNumberGenerator.Create();
            do
            {
                rng.GetBytes(array);
                var ulongSpan = MemoryMarshal.Cast<byte, ulong>(array);
                _s3 = ulongSpan[3];
                _s0 = ulongSpan[0];
                _s1 = ulongSpan[1];
                _s2 = ulongSpan[2];
            } while (_s0 == 0 && _s1 == 0 && _s2 == 0 && _s3 == 0);
#endif
        }

        public ulong NextUInt64()
        {
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;

#if NETCOREAPP3_0_OR_GREATER
            ulong result = BitOperations.RotateLeft(s1 * 5, 7) * 9;
#else
            ulong result = s1 * 5;
            result = s1 << 7 ^ s1 >> -7;
            result *= 9;
#endif
            ulong t = s1 << 17;

            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;

            s2 ^= t;

#if NETCOREAPP3_0_OR_GREATER
            s3 = BitOperations.RotateLeft(s3, 45);
#else
            s3 = s3 << 45 ^ s3 >> -45;
#endif

            _s0 = s0;
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;

            return result;
        }

        private static ulong BigMul(ulong a, ulong b, out ulong lo)
        {
#if NET5_0_OR_GREATER
            return Math.BigMul(a, b, out lo);
#else
            uint alo = (uint)a;
            uint ahi = (uint)(a >> 32);
            uint blo = (uint)b;
            uint bhi = (uint)(b >> 32);

            ulong mulo = (ulong)alo * blo;
            ulong mid1 = (ulong)ahi * blo;
            ulong mid2 = (ulong)alo * bhi;
            ulong muhi = (ulong)ahi * bhi;

            lo = mulo + (mid1 << 32) + (mid2 >> 32);
            return muhi + (mid1 >> 32) + (mid2 >> 32) + (((mid1 & ~0u) + (mid2 & ~0u) + (mulo >> 32)) >> 32);
#endif
        }

        public ulong NextUInt64(ulong maxExclusive)
        {
            ulong r = NextUInt64();
            ulong rhi = BigMul(r, maxExclusive, out ulong rlo);

            if (rlo < maxExclusive)
            {
                ulong mod = (0 - maxExclusive) % maxExclusive;
                while (rlo < mod)
                {
                    r = NextUInt64();
                    rhi = BigMul(r, maxExclusive, out rlo);
                }
            }

            return rhi;
        }

        // for details on the algorithm, see https://github.com/dotnet/runtime/pull/111015
        public void Shuffle<T>(Span<T> values)
        {
            ulong bound = 2432902008176640000;

            int nextIndex = Math.Min(20, values.Length);

            for (int i = 1; i < values.Length;)
            {
                ulong r = NextUInt64();
                ulong rbound = r * bound;
                if (rbound > 0 - bound)
                {
                    ulong sum, carry;
                    do
                    {
                        ulong t = NextUInt64();
                        ulong thi = BigMul(t, bound, out ulong tlo);
                        sum = rbound + thi;
                        carry = sum < rbound ? 1ul : 0ul;
                        rbound = tlo;
                    } while (sum == ~0ul);
                    r += carry;
                }

                for (int m = i; m < nextIndex; m++)
                {
                    int index = (int)BigMul(r, (ulong)(m + 1), out r);

                    T temp = values[m];
                    values[m] = values[index];
                    values[index] = temp;
                }

                i = nextIndex;

                bound = (ulong)(i + 1);
                for (nextIndex = i + 1; nextIndex < values.Length; nextIndex++)
                {
                    if (BigMul(bound, (ulong)(nextIndex + 1), out var newbound) == 0)
                    {
                        bound = newbound;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void PartialShuffle<T>(Span<T> values, int count)
        {
            count = Math.Min(count, values.Length);

            for (int i = 0; i < count;)
            {
                ulong bound = (ulong)(values.Length - i);
                int nextIndex;
                if (bound <= 20)
                {
                    bound = 2432902008176640000;
                    nextIndex = count;
                }
                else
                {
                    for (nextIndex = i + 1; nextIndex < count; nextIndex++)
                    {
                        if (BigMul(bound, (ulong)(values.Length - nextIndex), out var newbound) == 0)
                        {
                            bound = newbound;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                ulong r = NextUInt64();
                ulong rbound = r * bound;
                if (rbound > 0 - bound)
                {
                    ulong sum, carry;
                    do
                    {
                        ulong t = NextUInt64();
                        ulong thi = BigMul(t, bound, out ulong tlo);
                        sum = rbound + thi;
                        carry = sum < rbound ? 1ul : 0ul;
                        rbound = tlo;
                    } while (sum == ~0ul);
                    r += carry;
                }

                for (int m = i; m < nextIndex; m++)
                {
                    int index = m + (int)BigMul(r, (ulong)(values.Length - m), out r);

                    T temp = values[m];
                    values[m] = values[index];
                    values[index] = temp;
                }

                i = nextIndex;
            }
        }
    }

    [ThreadStatic]
    private static Xoshiro256StarStar? s_Shared;
    private static Xoshiro256StarStar Shared => s_Shared ??= new();
}
