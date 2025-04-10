using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new();
    private static bool applicationIsQuitting;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} is null because application is quitting.");
                return null;
            }


            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debug.LogError($"[Singleton] Multiple instances of {typeof(T)} found!");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        var singleton = new GameObject($"(singleton) {typeof(T)}");
                        _instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);

                        Debug.Log($"[Singleton] An instance of {typeof(T)} was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log($"[Singleton] Using existing instance: {_instance.gameObject.name}");
                    }
                }

                return _instance;
            }
        }
    }

    protected void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} removed.");
        }
        else
        {
            _instance = this as T;
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}