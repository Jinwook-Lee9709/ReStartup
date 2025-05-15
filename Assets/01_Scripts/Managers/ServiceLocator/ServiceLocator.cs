using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ServiceLocator : Singleton<ServiceLocator>
{
    private readonly SerializedDictionary<Type, object> globalServices = new();
    private readonly Dictionary<Type, object> sceneServices = new();

    public void Awake()
    {
        base.Awake();
        RegisterGlobalService(new SceneController());
        //ForTest
        var obj = GameObject.FindWithTag(Strings.GameManagerTag);
        if (obj == null)
        {
            Debug.LogWarning("GameManager object not found in the scene!");
            return;
        }

        Instance.RegisterSceneService(obj.GetComponent<GameManager>());
        //ForTest
    }

    public void RegisterGlobalService<T>(T service)
    {
        var type = typeof(T);
        if (globalServices.ContainsKey(type))
            return;
        globalServices.Add(typeof(T), service);
    }

    public void RegisterSceneService<T>(T service)
    {
        var type = typeof(T);
        if (sceneServices.ContainsKey(type))
            return;
        sceneServices.Add(typeof(T), service);
    }

    public T GetGlobalService<T>() where T : class
    {
        var type = typeof(T);
        if (!globalServices.ContainsKey(type))
            return null;
        return (T)globalServices[type];
    }

    public T GetSceneService<T>() where T : class
    {
        var type = typeof(T);
        if (!sceneServices.ContainsKey(type))
            return null;
        return (T)sceneServices[type];
    }

    public void ClearSceneServices()
    {
        sceneServices.Clear();
    }

    public void UnRegisterSceneService<T>()
    {
        var type = typeof(T);
        if (!sceneServices.ContainsKey(type))
            return;
        sceneServices.Remove(type);
    }
}