using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager : Singleton<ResourceManager>
{
    private void Awake()
    {
        base.Awake();
        Addressables.LoadAssetsAsync<Object>("Stage1", null);
    }
}
