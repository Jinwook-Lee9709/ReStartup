using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public class ObjectPoolManager
{
    private readonly Dictionary<Type, ObjectPool<GameObject>> pools = new();
    private Transform poolParent;

    public void Init(Transform poolParent)
    {
        this.poolParent = poolParent;
    }

    public void CreatePool<T>(T component, Action<T>onGet = null, Action<T>onRelease = null) where T : Component, IPoolable
    {
        var type = typeof(T);
        var localParent = new GameObject(type.ToString()).transform;
        localParent.SetParent(poolParent);

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
                var instance = GameObject.Instantiate(original, localParent);
                if (instance.TryGetComponent<IPoolable>(out var pooledObject))
                {
                    pooledObject.SetPool(newPool);
                }
                instance.SetActive(false);
                return instance;
            },
            obj =>
            {
                if (onGet != null)
                {
                    onGet(obj.GetComponent<T>());
                }
                obj.SetActive(true);
            },
            obj =>
            {
                if (onRelease != null)
                {
                    onRelease(obj.GetComponent<T>());
                }
                obj.SetActive(false);
                obj.transform.SetParent(localParent);
            },
            obj => Debug.Log($"{type.Name} destroyed"),
            true,
            10,
            100);
        pools.Add(type, newPool);
    }

    public T GetObjectFromPool<T>() where T : Component, IPoolable
    {
        var type = typeof(T);
        if (!pools.TryGetValue(type, out var pool))
        {
            Debug.LogError($"Can't find {type.Name} Pool");
            return null;
        }

        return pool.Get().GetComponent<T>();
    }


    public void ClearPool()
    {
        foreach (var pool in pools)
        {
            pool.Value.Clear();
        }
    }
}