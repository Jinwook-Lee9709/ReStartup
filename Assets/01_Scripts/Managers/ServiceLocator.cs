using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceLocator : Singleton<ServiceLocator>
{
    private Dictionary<Type, object> globalServices = new();
    private Dictionary<Type, object> sceneServices = new();

    public void Awake()
    {
        base.Awake();
        RegisterGlobalService(new SceneController());
        //ForTest
        var obj = GameObject.FindWithTag(Strings.GameManagerTag);
        if (obj == null)
        {
            Debug.LogError("GameManager object not found in the scene!");
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
