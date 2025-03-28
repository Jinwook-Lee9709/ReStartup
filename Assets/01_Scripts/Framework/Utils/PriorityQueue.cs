using System;
using System.Collections.Generic;

internal class PriorityQueue<TElement, TPriority>
{
    private readonly Comparer<TPriority> comparer;
    private (TPriority, TElement)[] arr = new (TPriority, TElement)[4];

    public PriorityQueue()
    {
        comparer = Comparer<TPriority>.Default;
    }

    public PriorityQueue(Comparer<TPriority> comparer)
    {
        this.comparer = comparer;
    }

    public int Count { get; private set; }

    public void Enqueue(TElement element, TPriority priority)
    {
        var tempIndex = ++Count;
        if (tempIndex >= arr.Length) ExpandBuf();
        arr[tempIndex] = (priority, element);

        while (tempIndex > 1)
        {
            var parentIndex = tempIndex / 2;
            if (comparer.Compare(arr[tempIndex].Item1, arr[parentIndex].Item1) > 0) return;
            Swap(tempIndex, parentIndex);
            tempIndex = parentIndex;
        }
    }

    public TElement Dequeue()
    {
        if (Count == 0) throw new InvalidOperationException("큐에 값이 없습니다");
        arr[0] = arr[1];
        arr[1] = default;
        if (Count == 1)
        {
            Count = 0;
            return arr[0].Item2;
        }

        var tempIndex = 1;
        arr[tempIndex] = arr[Count];
        arr[Count] = default;
        Count--;
        while (tempIndex * 2 <= Count)
        {
            var leftChildIndex = tempIndex * 2;
            var rightChildIndex = tempIndex * 2 + 1;

            if (leftChildIndex > Count) return arr[0].Item2;
            if (rightChildIndex > Count)
            {
                if (comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1) > 0)
                    Swap(tempIndex, leftChildIndex);
                return arr[0].Item2;
            }

            var lCompare = comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1);
            var rCompare = comparer.Compare(arr[tempIndex].Item1, arr[rightChildIndex].Item1);
            if (lCompare < 0 != rCompare < 0)
            {
                var swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0
                    ? leftChildIndex
                    : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;
            }
            else if (lCompare > 0 && rCompare > 0)
            {
                var swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0
                    ? leftChildIndex
                    : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;
            }
            else
            {
                return arr[0].Item2;
            }
        }

        return arr[0].Item2;
    }

    public bool TryDequeue(out TElement element, out TPriority priority)
    {
        if (Count == 0)
        {
            element = default;
            priority = default;
            return false;
        }

        arr[0] = arr[1];
        arr[1] = default;
        if (Count == 1)
        {
            Count = 0;
            return MakeSendBuf(out element, out priority);
        }

        var tempIndex = 1;
        arr[tempIndex] = arr[Count];
        arr[Count] = default;
        Count--;
        while (tempIndex < Count)
        {
            var leftChildIndex = tempIndex * 2;
            var rightChildIndex = tempIndex * 2 + 1;

            if (leftChildIndex > Count) return MakeSendBuf(out element, out priority);
            if (rightChildIndex > Count)
            {
                if (comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1) > 0)
                    Swap(tempIndex, leftChildIndex);
                return MakeSendBuf(out element, out priority);
            }

            var lCompare = comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1);
            var rCompare = comparer.Compare(arr[tempIndex].Item1, arr[rightChildIndex].Item1);
            if (lCompare > 0 != rCompare > 0)
            {
                var swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0
                    ? leftChildIndex
                    : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;
            }
            else if (lCompare > 0 && rCompare > 0)
            {
                var swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0
                    ? leftChildIndex
                    : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;
            }
            else
            {
                return MakeSendBuf(out element, out priority);
            }
        }

        return MakeSendBuf(out element, out priority);
    }

    private bool MakeSendBuf(out TElement element, out TPriority priority)
    {
        element = arr[0].Item2;
        priority = arr[0].Item1;
        return true;
    }

    public TElement Peek()
    {
        if (Count == 0) throw new InvalidOperationException("큐에 값이 없습니다");
        return arr[1].Item2;
    }

    public bool TryPeek(out TElement element, out TPriority priority)
    {
        if (Count == 0)
        {
            element = default;
            priority = default;
            return false;
        }

        element = arr[1].Item2;
        priority = arr[1].Item1;
        return true;
    }

    public void Clear()
    {
        Array.Clear(arr, 0, arr.Length);
        Count = 0;
    }

    private void Swap(int i, int j)
    {
        var temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    private void ExpandBuf()
    {
        Array.Resize(ref arr, arr.Length * 2);
    }
}