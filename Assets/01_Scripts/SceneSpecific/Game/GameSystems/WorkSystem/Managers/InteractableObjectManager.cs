using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableObjectManager<T> where T : class, IComparable<T>
{
    private SortedSet<T> availableObjects = new();
    private HashSet<T> occupiedObjects = new(); 
    
    public bool IsAvailableObjectExist => availableObjects.Count > 0;
    
    public event Action ObjectAvailableEvent; 
    
    public T GetAvailableObject()
    {
        if(availableObjects.Count == 0)
            return null;
        T obj = Dequeue();
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
        if(availableObjects.Count == 0)
            return null;
        T obj = availableObjects.Min;
        availableObjects.Remove(obj);
        occupiedObjects.Add(obj);
        return obj;
    }
    
}
