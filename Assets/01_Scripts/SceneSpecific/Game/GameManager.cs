using System;
using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static readonly string FoodObjectId = "";
    
    [SerializeField] private Transform poolParent;
    [SerializeField] private NavMeshSurface surface2D;
    [FormerlySerializedAs("mapSpriteLocator")] [FormerlySerializedAs("mapPivotLocator")] [SerializeField] private MapRendererLocator mapRendererLocator;

    private ThemeIds currentTheme;
    public ThemeIds CurrentTheme => currentTheme;
    public FoodUpgradeDataManager FoodUpgradeData { get; private set; }
    public ObjectPoolManager ObjectPoolManager { get; private set; }
    public WorkManager WorkManager { get; private set; }
    public WorkerManager WorkerManager { get; private set; }
    public WorkFlowController WorkFlowController { get; private set; }
    public WorkStationManager WorkStationManager { get; private set; }
    public ObjectPivotManager ObjectPivotManager { get; private set; }
    public InteriorManager InteriorManager { get; private set; }
    public EmployeeManager EmployeeManager { get; private set; }
    public MissionManager MissionManager { get; private set; }
    public RankSystemManager rankSystemManager;

    public Alarm alarm;
    public ConsumerManager consumerManager;
    public FoodManager foodManager;
    public BuffManager buffManager;
    public UiManager uiManager;
    public TutorialManager tutorialManager;
    #region InitializeClasses
    private void Awake()
    {
        currentTheme = (ThemeIds)PlayerPrefs.GetInt("Theme", 1);
        ServiceLocator.Instance.RegisterSceneService(this);
        
        InitDictionaries();
        
        InitFoodUpgradeDataManager();
        InitObjectPoolManager();
        InitWorkManagers();
        InitUIManagers();
        
        //EmployeeManager Must Init after InitDictionary
        InitEmployeeManager();
        
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
        
        ObjectPivotManager.Init(this, currentTheme, mapRendererLocator);
        WorkManager.Init(WorkerManager, alarm);
        WorkerManager.Init(WorkManager);
        WorkFlowController.Init(this, WorkManager);
        WorkStationManager.Init(this, WorkFlowController, ObjectPivotManager, surface2D);

    }

    private void InitUIManagers()
    {
        InteriorManager = new InteriorManager();
        InteriorManager.Init(this);
        MissionManager = new MissionManager();
        MissionManager.Init(this);
    }
    #endregion
 
    public void Start()
    {
        MissionManager.Start();
        WorkStationManager.BakeNavMesh();
        InteriorManager.Start();
        WorkerManager.Start();
        EmployeeManager.Start();
        AudioManager.Instance.Init();
    }

    public void Update()
    {
        WorkManager.UpdateWorkManager(Time.deltaTime);
    }

    #region InitGameScene

    private void InitGameScene()
    {
        InitInteractableObject();
    }

    private void InitInteractableObject()
    {
        InitFoodPickupCounter();
        InitTrayReturnCounter();
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

    //ForTest
    private void InitDictionaries()
    {
        InitInteriorDictionary();
        InitEmployeeDictionary();
        InitCookWareDictionary();
        InitFoodDictionary();
    }
    private void InitInteriorDictionary()
    {
        var interiorDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        var table = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        var list = table.Where(x=>x.RestaurantType.Equals((int)currentTheme)).ToList();
        
        foreach (var data in list)
        {
            interiorDictionary.TryAdd(data.InteriorID, 0);
        }
    }
    
    private void InitEmployeeDictionary()
    {
        var employeeDictionary = UserDataManager.Instance.CurrentUserData.EmployeeSaveData;
        var table = DataTableManager.Get<EmployeeDataTable>(DataTableIds.Employee.ToString());
        var list = table.Where(x=>x.Theme.Equals((int)currentTheme)).ToList();
        foreach (var data in list)
        {
            if (!employeeDictionary.ContainsKey(data.StaffID))
            {
                EmployeeSaveData buffer = new EmployeeSaveData();
                buffer.id = data.StaffID;
                buffer.level = 0;
                buffer.theme = (ThemeIds)data.Theme;
                buffer.remainHp = data.Health;
                buffer.remainHpDecreaseTime = 0f;
                employeeDictionary.TryAdd(buffer.id, buffer);
            }
        }
    }

    private void InitCookWareDictionary()
    {
        var cookwareDictionary = UserDataManager.Instance.CurrentUserData.CookWareUnlock;
        cookwareDictionary.Clear();
        var dict = new Dictionary<CookwareType, int>();
        var table = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var list = table.Where(x=>x.RestaurantType == (int)currentTheme).Select(x=>x.CookwareType).Distinct().ToList();
        foreach (var type in list)
        {
            dict.Add(type, 0);
        }
        cookwareDictionary[currentTheme] = dict;
    }

    private void InitFoodDictionary()
    {
        var foodDictionary = UserDataManager.Instance.CurrentUserData.FoodSaveData;
        var table = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString());
        var list = table.Where(x=>x.Type.Equals((int)currentTheme)).ToList();


        foreach (var data in list)
        {
            if (!foodDictionary.ContainsKey(data.FoodID))
            {
                FoodSaveData buffer = new FoodSaveData();
                buffer.id = data.FoodID;
                buffer.level = 0;
                buffer.theme = (ThemeIds)data.Type;
                buffer.sellCount = 0;
                foodDictionary.TryAdd(buffer.id, buffer);
            }
        }
    }
    
    
    private void InitEmployeeManager()
    {
        EmployeeManager = new EmployeeManager();
        EmployeeManager.Init(this);
        
    }

    
    #endregion
    
    
    //For Test
    [VInspector.Button]
    public void AddTrash(ObjectArea area)
    {
        WorkFlowController.CreateTrash(area);
    }

    public void OnDestroy()
    {
        InteriorManager.Dispose();
        WorkerManager.Dispose();
        WorkManager.Dispose();
        EmployeeManager.Dispose();
    }
    //For Test
}