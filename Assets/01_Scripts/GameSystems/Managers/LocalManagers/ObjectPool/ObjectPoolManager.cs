using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    private readonly Dictionary<Type, ObjectPool<GameObject>> pools = new();

    public void CreatePool<T>(T component) where T : Component, IPoolable
    {
        var type = typeof(T);
        if (pools.ContainsKey(type))
        {
            Debug.LogWarning("Pool already exists");
            return;
        }

        var original = component.gameObject;
        
        ObjectPool<GameObject> newPool = null;
        
        newPool = new ObjectPool<GameObject>(
            () =>
            {
                var instance = Instantiate(original);
                if (instance.TryGetComponent<IPoolable>(out var pooledObject))
                {
                    pooledObject.SetPool(newPool);
                }
                instance.SetActive(false);
                return instance;
            },
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            obj => Debug.Log($"{type.Name} destroyed"),
            true,
            10,
            100);
        pools.Add(type, newPool);
    }

    public GameObject GetObjectFromPool<T>() where T : Component, IPoolable
    {
        var type = typeof(T);
        if (!pools.TryGetValue(type, out var pool))
        {
            Debug.LogError($"Can't find {type.Name} Pool");
            return null;
        }

        return pool.Get();
    }


    public void ClearPool()
    {
        foreach (var pool in pools)
        {
            pool.Value.Clear();
        }
    }
}