using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager
{
    private readonly Dictionary<Type, ObjectPool<GameObject>> pools = new();

    public void CreatePool(Component component)
    {
        var type = component.GetType();
        if (pools.ContainsKey(type))
        {
            Debug.LogWarning("Pool already exists");
            return;
        }

        var original = component.gameObject;

        var newPool = new ObjectPool<GameObject>(
            () =>
            {
                var instance = GameObject.Instantiate(original);
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
}