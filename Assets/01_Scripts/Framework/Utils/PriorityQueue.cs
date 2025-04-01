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

    public bool TryRemoveAt(Predicate<TElement> condition,out TElement removedElement)
    {
        for (int i = 0; i < Count; i++)
        {
            if (condition(arr[i].Item2))
            {
                removedElement = arr[i].Item2;
                RemoveAt(i);
                return true;
            }
        }
        
        removedElement = default;
        return false; // 조건에 맞는 요소를 찾지 못함
    }
    
    private void RemoveAt(int index)
    {
        if (index < 1 || index > Count) throw new ArgumentOutOfRangeException(nameof(index));
    
        arr[index] = arr[Count]; // 마지막 요소를 제거된 위치로 이동
        arr[Count] = default;    // 배열에서 마지막 요소 제거
        Count--;

        // 힙 재구성: 부모에서 자식으로 또는 자식에서 부모로
        HeapifyDown(index); // 아래로 내려가는 조정
        HeapifyUp(index);   // 위로 올라가는 조정 (필요 시)
    }

    private void HeapifyDown(int index)
    {
        int leftChild, rightChild, smallerChild;
        while (index * 2 <= Count) // 왼쪽 자식이 있을 때
        {
            leftChild = index * 2;
            rightChild = leftChild + 1;
            smallerChild = rightChild <= Count && comparer.Compare(arr[rightChild].Item1, arr[leftChild].Item1) < 0
                ? rightChild
                : leftChild;

            if (comparer.Compare(arr[index].Item1, arr[smallerChild].Item1) <= 0) break;

            Swap(index, smallerChild);
            index = smallerChild;
        }
    }

    private void HeapifyUp(int index)
    {
        while (index > 1)
        {
            int parentIndex = index / 2;
            if (comparer.Compare(arr[index].Item1, arr[parentIndex].Item1) >= 0) break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
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
        (arr[i], arr[j]) = (arr[j], arr[i]);
    }

    private void ExpandBuf()
    {
        Array.Resize(ref arr, arr.Length * 2);
    }
}