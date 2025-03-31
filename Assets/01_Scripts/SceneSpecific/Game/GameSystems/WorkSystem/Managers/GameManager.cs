using System;
using UnityEngine;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static readonly string FoodObjectId = "";


    [SerializeField] private ThemeIds currentTheme;

    public ThemeIds CurrentTheme => currentTheme;
    public FoodUpgradeDataManager FoodUpgradeData { get; private set; }
    public ObjectPoolManager ObjectPoolManager { get; private set; }
    public WorkerManager WorkerManager { get; private set; }

    private void Awake()
    {
        FoodUpgradeData = new FoodUpgradeDataManager();
        ObjectPoolManager = new ObjectPoolManager();

        WorkerManager = FindObjectOfType<WorkerManager>();

        FoodUpgradeData.Init();

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