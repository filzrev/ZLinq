using System.Buffers;

namespace ZLinq.Internal;

[StructLayout(LayoutKind.Auto)]
internal struct ValueRingBuffer<T>(int capacity) : IDisposable
{
    public T[]? Buffer = ArrayPool<T>.Shared.Rent(capacity);
    public readonly int Capacity = capacity;
    int head = 0;
    public int Count = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        Buffer![head] = item;
        head = (head + 1) % Capacity;
        Count = Math.Min(Count + 1, Capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue(out T item)
    {
        if (Count == 0)
        {
            item = default!;
            return false;
        }

        var tail = (head - Count + Capacity) % Capacity;
        ref var tailRef = ref Buffer![tail];
        item = tailRef;
        tailRef = default!;
        Count--;
        return true;
    }


    public void Dispose()
    {
        if (Buffer != null)
        {
            // Clear the buffer if T contains references
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Buffer.AsSpan(0, Capacity).Clear();
            }

            ArrayPool<T>.Shared.Return(Buffer);
            Buffer = null;
        }
    }
}
