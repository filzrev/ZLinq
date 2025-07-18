using System.Buffers;

namespace ZLinq.Internal;

[StructLayout(LayoutKind.Auto)]
internal class RentedRingBuffer<T>(int capacity) : IDisposable
{
    public T[]? Buffer = ArrayPool<T>.Shared.Rent(4);
    public readonly int Capacity = capacity;
    int head = 0;
    public int Count = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        Buffer![head] = item;
        var newHead = (head + 1);
        if (newHead >= Buffer.Length)
        {
            Expand();
        }

        head = newHead % Capacity;
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    void Expand()
    {
        if (Buffer == null || Buffer.Length < Capacity)
        {
            var newBuffer = ArrayPool<T>.Shared.Rent(Capacity);
            if (Buffer != null)
            {
                var prevSpan = new Span<T>(Buffer, 0, Count);
                prevSpan.CopyTo(newBuffer);
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    prevSpan.Clear();
                }

                ArrayPool<T>.Shared.Return(Buffer);
            }

            Buffer = newBuffer;
            head = Count; // Reset head to the end of the current items
        }
    }

    public void Dispose()
    {
        if (Buffer != null)
        {
            // Clear the buffer if T contains references
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Buffer.AsSpan(0, Count).Clear();
            }

            ArrayPool<T>.Shared.Return(Buffer);
            Buffer = null;
        }
    }
}
