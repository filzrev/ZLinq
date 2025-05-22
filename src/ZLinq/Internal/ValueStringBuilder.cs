namespace ZLinq.Internal;

// ValueStringBuilder is SegmentedArrayProvider char extension
internal unsafe ref struct ValueStringBuilder // TODO: internal
{
#if NET8_0_OR_GREATER
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "FastAllocateString")]
    static extern string FastAllocateString(string _, int length);
#else
    static string FastAllocateString(string _, int length) => new string('\0', length);
#endif

    Span<char> chars;
    int currentPosition;

    SegmentedArrayProvider<char> arrayProvider;

    public ValueStringBuilder(Span<char> initialBuffer)
    {
        arrayProvider = new(initialBuffer);
        chars = initialBuffer;
        currentPosition = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char value)
    {
    AGAIN:
        if ((uint)currentPosition < (uint)chars.Length)
        {
            chars[currentPosition] = value;
            currentPosition += 1;
        }
        else
        {
            FlushCurrentChars();
            goto AGAIN;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? value)
    {
        if (value == null) return;

        if (currentPosition > chars.Length - value.Length)
        {
            // partial-copy
            Append(value.AsSpan());
        }
        else
        {
            // full-copy
#if NET8_0_OR_GREATER
            value.CopyTo(chars.Slice(currentPosition));
#else
            value.AsSpan().CopyTo(chars.Slice(currentPosition));
#endif
            currentPosition += value.Length;
        }
    }

    public void Append(scoped ReadOnlySpan<char> value)
    {
    AGAIN:
        if (value.TryCopyTo(chars.Slice(currentPosition)))
        {
            currentPosition += value.Length;
        }
        else
        {
            var dest = chars.Slice(currentPosition);
            value.Slice(0, dest.Length).TryCopyTo(dest);
            value = value.Slice(dest.Length);
            currentPosition += dest.Length;

            FlushCurrentChars();
            goto AGAIN;
        }
    }

    public unsafe void Append<T>(T value)
    {
        if (typeof(T) == typeof(string))
        {
            Append(Unsafe.As<T, string>(ref value));
        }
#if NETSTANDARD2_1 || NET8_0_OR_GREATER
        else if (typeof(T) == typeof(byte))
        {
            const int CharMaxLength = 3;
            if (Unsafe.As<T, byte>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, byte>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(sbyte))
        {
            const int CharMaxLength = 4;
            if (Unsafe.As<T, sbyte>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, sbyte>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(short))
        {
            const int CharMaxLength = 6;
            if (Unsafe.As<T, short>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, short>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(ushort))
        {
            const int CharMaxLength = 5;
            if (Unsafe.As<T, ushort>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, ushort>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(int))
        {
            const int CharMaxLength = 11;
            if (Unsafe.As<T, int>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, int>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(uint))
        {
            const int CharMaxLength = 10;
            if (Unsafe.As<T, uint>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, uint>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(long))
        {
            const int CharMaxLength = 20;
            if (Unsafe.As<T, long>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, long>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(ulong))
        {
            const int CharMaxLength = 20;
            if (Unsafe.As<T, ulong>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, ulong>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(float))
        {
            const int CharMaxLength = 128; // reserved space for unknown culture specified format
            if (Unsafe.As<T, float>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, float>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(double))
        {
            const int CharMaxLength = 128; // reserved space for unknown culture specified format
            if (Unsafe.As<T, double>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, double>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
        else if (typeof(T) == typeof(decimal))
        {
            const int CharMaxLength = 128; // reserved space for unknown culture specified format
            if (Unsafe.As<T, decimal>(ref value).TryFormat(chars.Slice(currentPosition), out var charsWritten))
            {
                currentPosition += charsWritten;
            }
            else
            {
                Span<char> temp = stackalloc char[CharMaxLength];
                Unsafe.As<T, decimal>(ref value).TryFormat(temp, out charsWritten);
                Append(temp.Slice(0, charsWritten));
            }
        }
#endif
        else
        {
            // NETATANDARD2_0 has no TryFormat so always boxed.
            // If Enum, we do not have Enum.TryFormatUnconstrained so always boxed in NETSTANDARD2_1.

#if NET8_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                if (((ISpanFormattable)value).TryFormat(chars.Slice(currentPosition), out charsWritten, default, null))
                {
                    currentPosition += charsWritten;
                    return;
                }
            }
#endif

            string? s;
            if (value is IFormattable)
            {
                s = ((IFormattable)value).ToString(format: null, formatProvider: null);
            }
            else
            {
                s = value?.ToString();
            }

            if (s is not null)
            {
                Append(s);
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void FlushCurrentChars()
    {
        arrayProvider.Advance(currentPosition);
        chars = arrayProvider.GetSpan();
        currentPosition = 0;
    }

    public unsafe string ToStringAndClear()
    {
        arrayProvider.Advance(currentPosition);

        var length = arrayProvider.Count;

        if (arrayProvider.IsInitialBufferOnly)
        {

            var str = chars.Slice(0, length);
#if !NETSTANDARD2_0
            return new string(str);
#else
            fixed (char* p = str)
            {
                return new string(p, 0, str.Length);
            }
#endif
        }
        else
        {
            var str = FastAllocateString(null!, length);

            fixed (char* p = str)
            {
                var span = new Span<char>(p, length);
                arrayProvider.CopyToAndClear(span);
            }

            return str;
        }
    }
}

//internal ref struct ValueStringBuilder
//{
//    char[]? _arrayToReturnToPool;
//    Span<char> _chars;
//    int _pos;

//    public ValueStringBuilder(Span<char> initialBuffer)
//    {
//        _arrayToReturnToPool = null;
//        _chars = initialBuffer;
//        _pos = 0;
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void Append(char value)
//    {
//        int pos = _pos;
//        Span<char> chars = _chars;
//        if ((uint)pos < (uint)chars.Length)
//        {
//            chars[pos] = value;
//            _pos = pos + 1;
//        }
//        else
//        {
//            GrowAndAppend(value);
//        }
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void Append(string? value)
//    {
//        if (value == null) return;

//        int pos = _pos;
//        if (pos > _chars.Length - value.Length)
//        {
//            Grow(value.Length);
//        }

//#if NET8_0_OR_GREATER
//        value.CopyTo(_chars.Slice(pos));
//#else
//        value.AsSpan().CopyTo(_chars.Slice(pos));
//#endif

//        _pos += value.Length;
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void Append(ReadOnlySpan<char> value)
//    {
//        if (value.TryCopyTo(_chars.Slice(_pos)))
//        {
//            _pos += value.Length;
//        }
//        else
//        {
//            GrowThenCopyString(value);
//        }
//    }

//    public void Append<T>(T value)
//    {
//        if (typeof(T) == typeof(string))
//        {
//            Append(Unsafe.As<T, string>(ref value));
//        }
//#if NETSTANDARD2_1 || NET8_0_OR_GREATER
//        else if (typeof(T) == typeof(byte))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, byte>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(1);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(sbyte))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, sbyte>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(1);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(short))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, short>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(2);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(ushort))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, ushort>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(2);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(int))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, int>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(4);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(uint))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, uint>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(4);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(long))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, long>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(8);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(ulong))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, ulong>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(8);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(float))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, float>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(4);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(double))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, double>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(8);
//            }

//            _pos += charsWritten;
//            return;
//        }
//        else if (typeof(T) == typeof(decimal))
//        {
//            int charsWritten;
//            while (!(Unsafe.As<T, decimal>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//            {
//                Grow(16);
//            }

//            _pos += charsWritten;
//            return;
//        }
//#endif
//        else
//        {
//            // NETATANDARD2_0 has no TryFormat so always boxed.
//            // If Enum, we do not have Enum.TryFormatUnconstrained so always boxed in NETSTANDARD2_1.

//#if NET8_0_OR_GREATER
//            if (value is ISpanFormattable)
//            {
//                int charsWritten;
//                if (((ISpanFormattable)value).TryFormat(_chars.Slice(_pos), out charsWritten, default, null))
//                {
//                    _pos += charsWritten;
//                    return;
//                }
//            }
//#endif

//            string? s;
//            if (value is IFormattable)
//            {
//                s = ((IFormattable)value).ToString(format: null, formatProvider: null);
//            }
//            else
//            {
//                s = value?.ToString();
//            }

//            if (s is not null)
//            {
//                Append(s);
//            }
//        }
//    }

//    public string ToStringAndClear()
//    {
//#if NETSTANDARD2_1 || NET8_0_OR_GREATER
//        string result = new string(_chars.Slice(0, _pos));
//#else
//        string result;
//        unsafe
//        {
//            var txt = _chars.Slice(0, _pos);
//            fixed (char* p = txt)
//            {
//                result = new string(p, 0, txt.Length);
//            }
//        }
//#endif

//        // Dispose
//        char[]? toReturn = _arrayToReturnToPool;
//        this = default;
//        if (toReturn != null)
//        {
//            ArrayPool<char>.Shared.Return(toReturn, clearArray: false);
//        }

//        return result;
//    }

//    [MethodImpl(MethodImplOptions.NoInlining)]
//    void GrowAndAppend(char c)
//    {
//        Grow(1);
//        Append(c);
//    }

//    [MethodImpl(MethodImplOptions.NoInlining)]
//    private void GrowThenCopyString(ReadOnlySpan<char> value)
//    {
//        Grow(value.Length);
//        value.CopyTo(_chars.Slice(_pos));
//        _pos += value.Length;
//    }

//    // from dotnet/runtime ValueStringBuilder
//    [MethodImpl(MethodImplOptions.NoInlining)]
//    private void Grow(int additionalCapacityBeyondPos)
//    {
//        Debug.Assert(additionalCapacityBeyondPos > 0);
//        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

//        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

//        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
//        // to double the size if possible, bounding the doubling to not go beyond the max array length.
//        int newCapacity = (int)Math.Max(
//            (uint)(_pos + additionalCapacityBeyondPos),
//            Math.Min((uint)_chars.Length * 2, ArrayMaxLength));

//        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
//        // This could also go negative if the actual required length wraps around.
//        char[] poolArray = ArrayPool<char>.Shared.Rent(newCapacity);

//        _chars.Slice(0, _pos).CopyTo(poolArray);

//        char[]? toReturn = _arrayToReturnToPool;
//        _chars = _arrayToReturnToPool = poolArray;
//        if (toReturn != null)
//        {
//            ArrayPool<char>.Shared.Return(toReturn);
//        }
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void Dispose()
//    {
//        char[]? toReturn = _arrayToReturnToPool;
//        this = default;
//        if (toReturn != null)
//        {
//            ArrayPool<char>.Shared.Return(toReturn);
//        }
//    }
//}
