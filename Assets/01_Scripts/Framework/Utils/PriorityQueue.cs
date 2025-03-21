using System;
using System.Buffers;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

class PriorityQueue<TElement, TPriority>
{
    int count = 0;
    public int Count => count;
    private (TPriority, TElement)[] arr = new (TPriority, TElement)[4];
    Comparer<TPriority> comparer;

    public PriorityQueue()
    {
        this.comparer = Comparer<TPriority>.Default;
    }

    public PriorityQueue(Comparer<TPriority> comparer)
    {
        this.comparer = comparer;
    }

    public void Enqueue(TElement element, TPriority priority)
    {
        int tempIndex = ++count;
        if(tempIndex >= arr.Length)
        {
            ExpandBuf();
        }
        arr[tempIndex] = (priority, element);

        while(tempIndex > 1)
        {
            int parentIndex = tempIndex / 2;
            if(comparer.Compare(arr[tempIndex].Item1, arr[parentIndex].Item1) > 0)
            {
                return;
            }
            Swap(tempIndex, parentIndex);
            tempIndex = parentIndex;
        }
    }

    public TElement Dequeue()
    {
        if(count == 0)
        {
            throw new InvalidOperationException("큐에 값이 없습니다");
        }
        arr[0] = arr[1];
        arr[1] = default;
        if(count == 1)
        {
            count = 0;
            return arr[0].Item2;
        }
        int tempIndex = 1;
        arr[tempIndex] = arr[count];
        arr[count] = default;
        count--;
        while(tempIndex * 2 <= count)
        {
            int leftChildIndex = tempIndex * 2;
            int rightChildIndex = tempIndex * 2 + 1;

            if(leftChildIndex > count)
            {

                return arr[0].Item2;
            }
            if(rightChildIndex > count)
            {
                if(comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1) > 0)
                {
                    Swap(tempIndex, leftChildIndex);
                }
                return arr[0].Item2;
            }
            int lCompare = comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1);
            int rCompare = comparer.Compare(arr[tempIndex].Item1, arr[rightChildIndex].Item1);
            if(lCompare < 0 != rCompare < 0)
            {
                int swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0 ? leftChildIndex : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;

            }else if(lCompare > 0 && rCompare > 0)
            {
                int swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0 ? leftChildIndex : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;
            }else
            {
                return arr[0].Item2;
            }
        }
        return arr[0].Item2;
    }

    public bool TryDequeue(out TElement element, out TPriority priority)
    {
        if(count == 0)
        {
            element = default;
            priority = default;
            return false;
        }
        arr[0] = arr[1];
        arr[1] = default;
        if(count == 1)
        {
            count = 0;
            return MakeSendBuf(out element, out priority);
        }
        int tempIndex = 1;
        arr[tempIndex] = arr[count];
        arr[count] = default;
        count--;
        while(tempIndex < count)
        {
            int leftChildIndex = tempIndex * 2;
            int rightChildIndex = tempIndex * 2 + 1;

            if(leftChildIndex > count)
            {
                return MakeSendBuf(out element, out priority);
            }
            if(rightChildIndex > count)
            {
                if(comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1) > 0)
                {
                    Swap(tempIndex, leftChildIndex);
                }
                return MakeSendBuf(out element, out priority);
            }
            int lCompare = comparer.Compare(arr[tempIndex].Item1, arr[leftChildIndex].Item1);
            int rCompare = comparer.Compare(arr[tempIndex].Item1, arr[rightChildIndex].Item1);
            if(lCompare > 0 != rCompare > 0)
            { 
                int swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0 ? leftChildIndex : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;
            }else if(lCompare > 0 && rCompare > 0)
            {
                int swapIndex = comparer.Compare(arr[leftChildIndex].Item1, arr[rightChildIndex].Item1) < 0 ? leftChildIndex : rightChildIndex;
                Swap(tempIndex, swapIndex);
                tempIndex = swapIndex;
            }else
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
        if(count == 0)
        {
            throw new InvalidOperationException("큐에 값이 없습니다");
        }
        return arr[1].Item2;
    }

    public bool TryPeek(out TElement element, out TPriority priority)
    {
        if(count == 0)
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
        count = 0;
    }

    private void Swap(int i, int j)
    {
        (TPriority, TElement) temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    private void ExpandBuf()
    {
        Array.Resize<(TPriority, TElement)>(ref arr, arr.Length * 2);
    }
}