using System;
using UnityEngine;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static readonly string FoodObjectId = "";
    
    [SerializeField] private ThemeIds currentTheme;
    [SerializeField] private Transform poolParent;

    public ThemeIds CurrentTheme => currentTheme;
    public FoodUpgradeDataManager FoodUpgradeData { get; private set; }
    public ObjectPoolManager ObjectPoolManager { get; private set; }
    public WorkerManager WorkerManager { get; private set; }

    private void Awake()
    {
        FoodUpgradeData = new FoodUpgradeDataManager();
        FoodUpgradeData.Init();
        
        ObjectPoolManager = new ObjectPoolManager();
        if(poolParent == null)
            poolParent = new GameObject("Pool").transform;
        ObjectPoolManager.Init(poolParent);

        WorkerManager = FindObjectOfType<WorkerManager>();

        

        InitGameScene();
    }

    public void Start()
    {
        var dict = UserDataManager.Instance.CurrentUserData.FoodSaveData;
        foreach (var pair in dict) Debug.Log(pair.Key + " " + pair.Value.SellCount + " " + pair.Value.UpgradeLevel);
    }

    //Must Call in Last of Awake
    private void InitGameScene()
    {
        // Addressables.LoadAssetsAsync<FoodObject>()
        // objectPoolManager.CreatePool();
    }
}