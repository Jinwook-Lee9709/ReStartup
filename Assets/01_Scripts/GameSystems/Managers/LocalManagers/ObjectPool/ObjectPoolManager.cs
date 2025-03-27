using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager
{
    private Dictionary<Type, ObjectPool<GameObject>> pools = new();

    public void CreatePool(Component component)
    {
        Type type = component.GetType();
        if (pools.ContainsKey(type))
        {
            Debug.LogWarning("Pool already exists");
            return;
        }
        
        GameObject original = component.gameObject;
        
        ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject instance = GameObject.Instantiate(original);
                instance.SetActive(false);
                return instance;
            },
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Debug.Log($"{type.Name} destroyed"),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 100);
        pools.Add(type, newPool);
    }
    
}
