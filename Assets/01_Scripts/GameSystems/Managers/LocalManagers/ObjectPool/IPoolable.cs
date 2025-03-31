using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable
{
    void SetPool(ObjectPool<GameObject> pool);
    void Release();      
}
