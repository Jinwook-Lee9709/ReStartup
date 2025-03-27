using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static readonly string FoodObjectId = "";
    
    
    [SerializeField] private ThemeIds currentTheme;
    
    private FoodUpgradeDataManager foodUpgradeData;
    private ObjectPoolManager objectPoolManager;
    private WorkerManager workerManager;
    
    public ThemeIds CurrentTheme => currentTheme;
    public FoodUpgradeDataManager FoodUpgradeData => foodUpgradeData;
    public ObjectPoolManager ObjectPoolManager => objectPoolManager;
    public WorkerManager WorkerManager => workerManager;

    private void Awake()
    {
        foodUpgradeData = new FoodUpgradeDataManager();
        objectPoolManager = new ObjectPoolManager();

        workerManager = FindObjectOfType<WorkerManager>();
        
        foodUpgradeData.Init();

        InitGameScene();
    }
    
    public void Start()
    {
        var dict = UserDataManager.Instance.CurrentUserData.FoodSaveData;
        foreach (var pair in dict)
        {
            Debug.Log(pair.Key + " " + pair.Value.SellCount + " " + pair.Value.UpgradeLevel);
        }
    }

    //Must Call in Last of Awake
    private void InitGameScene()
    {
        // Addressables.LoadAssetsAsync<FoodObject>()
        // objectPoolManager.CreatePool();
    }
}
