using System;
using NavMeshPlus.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static readonly string FoodObjectId = "";
    
    [SerializeField] private Transform poolParent;
    [SerializeField] private NavMeshSurface surface2D;

    private ThemeIds currentTheme;
    public ThemeIds CurrentTheme => currentTheme;
    public FoodUpgradeDataManager FoodUpgradeData { get; private set; }
    public ObjectPoolManager ObjectPoolManager { get; private set; }
    public WorkManager WorkManager { get; private set; }
    public WorkerManager WorkerManager { get; private set; }
    public WorkFlowController WorkFlowController { get; private set; }
    public WorkStationManager WorkStationManager { get; private set; }
    public ObjectPivotManager ObjectPivotManager { get; private set; }

    public ConsumerManager consumerManager;
    #region InitializeClasses
    private void Awake()
    {
        currentTheme = (ThemeIds)PlayerPrefs.GetInt("Theme", 1);
        ServiceLocator.Instance.RegisterSceneService(this);

        InitFoodUpgradeDataManager();
        InitObjectPoolManager();
        InitWorkManagers();
        
        InitGameScene();
    }
    private void InitFoodUpgradeDataManager()
    {
        FoodUpgradeData = new FoodUpgradeDataManager();
        FoodUpgradeData.Init();
    }
    private void InitObjectPoolManager()
    {
        ObjectPoolManager = new ObjectPoolManager();
        if(poolParent == null)
            poolParent = new GameObject("Pool").transform;
        ObjectPoolManager.Init(poolParent);
    }
    private void InitWorkManagers()
    {
        ObjectPivotManager = new ObjectPivotManager();
        WorkManager = new WorkManager();
        WorkerManager = new WorkerManager();
        WorkFlowController = new WorkFlowController();
        WorkStationManager = new WorkStationManager();
        
        ObjectPivotManager.Init(currentTheme);
        WorkManager.Init(WorkerManager);
        WorkerManager.Init(WorkManager);
        WorkFlowController.Init(this, WorkManager);
        WorkStationManager.Init(WorkFlowController, ObjectPivotManager, surface2D);

    }
    #endregion
 
    public void Start()
    {
        WorkStationManager.BakeNavMesh();
        WorkerManager.Start();
    }

    #region InitGameScene

    private void InitGameScene()
    {
        InitInteractableObject();
    }

    private void InitInteractableObject()
    {
        InitCounter();
        InitFoodPickupCounter();
        InitTrayReturnCounter();
        InitSinkingStation();
    }

    private void InitCounter()
    {
        var counterPivot = ObjectPivotManager.GetCounterPivot();
        var handle = Addressables.InstantiateAsync(Strings.CounterName);
        handle.WaitForCompletion();
        var counter = handle.Result.GetComponent<CashierCounter>();
        counter.transform.SetParentAndInitialize(counterPivot);
        WorkFlowController.SetCashierCounter(counter);
    }

    private void InitFoodPickupCounter()
    {
        var pickupCounterPivots= ObjectPivotManager.GetPickupCounterPivots();
        for(int i = 0; i < pickupCounterPivots.Count; i++)
        {
            var handle = Addressables.InstantiateAsync(Strings.FoodPickupCounterName);
            handle.WaitForCompletion();
            var counter = handle.Result.GetComponent<FoodPickupCounter>();
            counter.transform.SetParentAndInitialize(pickupCounterPivots[i]);
            counter.SetId(i);
            WorkFlowController.AddFoodPickupCounter(counter);
        }
    }

    private void InitTrayReturnCounter()
    {
        var trayReturnCounterPivot = ObjectPivotManager.GetTrayReturnPivot();
        var handle = Addressables.InstantiateAsync(Strings.TrayReturnCounter);
        handle.WaitForCompletion();
        var counter = handle.Result.GetComponent<TrayReturnCounter>();
        counter.transform.SetParentAndInitialize(trayReturnCounterPivot);
        WorkFlowController.SetTrayReturnCounter(counter);
    }

    private void InitSinkingStation()
    {
        var sinkingStationPivot = ObjectPivotManager.GetSinkPivot();
        var handle = Addressables.InstantiateAsync(Strings.SinkingStation);
        handle.WaitForCompletion();
        var station = handle.Result.GetComponent<SinkingStation>();
        station.transform.SetParentAndInitialize(sinkingStationPivot);
        WorkFlowController.SetSinkingStation(station);
    }

    #endregion
    
}