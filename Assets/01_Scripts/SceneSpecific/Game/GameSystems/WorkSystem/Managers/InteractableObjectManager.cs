using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectManager<T> where T : class, IComparable<T>
{
    private readonly SortedSet<T> availableObjects = new();
    private readonly HashSet<T> occupiedObjects = new();

    public bool IsAvailableObjectExist => availableObjects.Count > 0;

    public int Count => availableObjects.Count;

    public event Action ObjectAvailableEvent;

    public void InsertObject(T obj)
    {
        availableObjects.Add(obj);
        ObjectAvailableEvent?.Invoke();
    }

    public T GetAvailableObject()
    {
        if (availableObjects.Count == 0)
            return null;
        var obj = Dequeue();
        return obj;
    }

    public bool ReturnObject(T obj)
    {
        if (!occupiedObjects.Contains(obj))
        {
            Debug.LogError("Object Return failed. Object not found in occupiedObjects");
            return false;
        }

        Enqueue(obj);
        ObjectAvailableEvent?.Invoke();
        return true;
    }

    public T Enqueue(T obj)
    {
        availableObjects.Add(obj);
        occupiedObjects.Remove(obj);
        return obj;
    }

    public T Dequeue()
    {
        if (availableObjects.Count == 0)
            return null;
        var obj = availableObjects.Min;
        availableObjects.Remove(obj);
        occupiedObjects.Add(obj);
        return obj;
    }
}