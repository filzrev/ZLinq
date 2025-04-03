#if !NET8_0_OR_GREATER

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace ZLinq.Internal;

// used for JoinToString, only needs ctor, AppendFormatted, AppendLiteral
internal ref struct DefaultInterpolatedStringHandler
{
    const int StringMaxLength = 0x3FFFFFDF;
    const int MinimumArrayPoolLength = 256;

    private readonly IFormatProvider? _provider;
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    public DefaultInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, Span<char> initialBuffer)
    {
        _provider = provider;
        _chars = initialBuffer;
        _arrayToReturnToPool = null;
        _pos = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<char> value)
    {
        if (value.TryCopyTo(_chars.Slice(_pos)))
        {
            _pos += value.Length;
        }
        else
        {
            GrowThenCopyString(value);
        }
    }

    public void AppendFormatted<T>(T value)
    {
        if (typeof(T) == typeof(string))
        {
            AppendFormatted(Unsafe.As<T, string>(ref value).AsSpan());
        }
#if NETSTANDARD2_1
        else if (typeof(T) == typeof(byte))
        {
            int charsWritten;
            while (!(Unsafe.As<T, byte>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(sbyte))
        {
            int charsWritten;
            while (!(Unsafe.As<T, sbyte>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(short))
        {
            int charsWritten;
            while (!(Unsafe.As<T, short>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(ushort))
        {
            int charsWritten;
            while (!(Unsafe.As<T, ushort>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(int))
        {
            int charsWritten;
            while (!(Unsafe.As<T, int>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(uint))
        {
            int charsWritten;
            while (!(Unsafe.As<T, uint>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(long))
        {
            int charsWritten;
            while (!(Unsafe.As<T, long>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(ulong))
        {
            int charsWritten;
            while (!(Unsafe.As<T, ulong>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(float))
        {
            int charsWritten;
            while (!(Unsafe.As<T, float>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(double))
        {
            int charsWritten;
            while (!(Unsafe.As<T, double>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
        else if (typeof(T) == typeof(decimal))
        {
            int charsWritten;
            while (!(Unsafe.As<T, decimal>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
#endif
        else
        {
            // NETATANDARD2_0 has no TryFormat so always boxed.
            // If Enum, we do not have Enum.TryFormatUnconstrained so always boxed in NETSTANDARD2_1.
            string? s;
            if (value is IFormattable)
            {
                s = ((IFormattable)value).ToString(format: null, _provider);
            }
            else
            {
                s = value?.ToString();
            }

            if (s is not null)
            {
                AppendFormatted(s.AsSpan());
            }
        }
    }

    public string ToStringAndClear()
    {
#if NETSTANDARD2_1
        string result = new string(_chars.Slice(0, _pos));
#else
        string result;
        unsafe
        {
            var txt = _chars.Slice(0, _pos);
            fixed (char* p = txt)
            {
                result = new string(p, 0, txt.Length);
            }
        }
#endif
        Clear();
        return result;
    }

    /// <summary>Clears the handler, returning any rented array to the pool.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // used only on a few hot paths
    internal void Clear()
    {
        char[]? toReturn = _arrayToReturnToPool;
        this = default; // defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopyString(ReadOnlySpan<char> value)
    {
        Grow(value.Length);
        value.CopyTo(_chars.Slice(_pos));
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow()
    {
        // This method is called when the remaining space in _chars isn't sufficient to continue
        // the operation.  Thus, we need at least one character beyond _chars.Length.  GrowCore
        // will handle growing by more than that if possible.
        GrowCore((uint)_chars.Length + 1);
    }

    [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    private void Grow(int additionalChars)
    {
        // This method is called when the remaining space (_chars.Length - _pos) is
        // insufficient to store a specific number of additional characters.  Thus, we
        // need to grow to at least that new total. GrowCore will handle growing by more
        // than that if possible.
        Debug.Assert(additionalChars > _chars.Length - _pos);
        GrowCore((uint)_pos + (uint)additionalChars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
    private void GrowCore(uint requiredMinCapacity)
    {
        // We want the max of how much space we actually required and doubling our capacity (without going beyond the max allowed length). We
        // also want to avoid asking for small arrays, to reduce the number of times we need to grow, and since we're working with unsigned
        // ints that could technically overflow if someone tried to, for example, append a huge string to a huge string, we also clamp to int.MaxValue.
        // Even if the array creation fails in such a case, we may later fail in ToStringAndClear.

        uint newCapacity = Math.Max(requiredMinCapacity, Math.Min((uint)_chars.Length * 2, StringMaxLength));
        int arraySize = (int)MathClamp(newCapacity, MinimumArrayPoolLength, int.MaxValue);

        char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
        _chars.Slice(0, _pos).CopyTo(newArray);

        char[]? toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = newArray;

        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint MathClamp(uint value, uint min, uint max)
    {
        if (min > max)
        {
            ThrowMinMaxException(min, max);
        }

        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }

        return value;
    }

    [DoesNotReturn]
    static void ThrowMinMaxException<T>(T min, T max)
    {
        throw new ArgumentException();
    }
}

#endif
