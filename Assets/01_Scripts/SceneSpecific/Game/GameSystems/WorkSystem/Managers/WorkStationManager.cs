using System;
using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkStationManager
{
    private static readonly string ChairIconID = "HallChair{0}";
    
    private readonly Dictionary<CookwareType, Dictionary<int, CookingStation>> cookingStations = new();
    private ObjectPivotManager objectPivotManager;
    private SinkingStation sinkingStation;
    private CashierCounter counter;
    private NavMeshSurface surface2D;
    private List<Transform> tablePivots;
    private Dictionary<ObjectArea, TrashCan> trashCans = new();

    private readonly Dictionary<int, Table> tables = new();
    private WorkFlowController workFlowController;
    private GameManager gameManager;

    public TrayReturnCounter TrayReturnCounter { get; }

    public int CurrentCookwareCount(CookwareType type)
    {
        return objectPivotManager.GetCookwarePivots(type).Count(x => x.childCount > 0);
    }

    public void Init(GameManager gameManager, WorkFlowController workFlowController, ObjectPivotManager objectPivotManager,
        NavMeshSurface surface2D)
    {
        this.gameManager = gameManager;
        this.workFlowController = workFlowController;
        this.objectPivotManager = objectPivotManager;
        this.surface2D = surface2D;

        tablePivots = objectPivotManager.GetTablePivots();

        InitContainer();
    }

    private void InitContainer()
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;

        var table = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var kvpList = table.GetSceneFoodDataList(currentTheme);
        var cookwareTypeList = kvpList.Select(pair => pair.Value.CookwareType).Distinct().ToList();
        foreach (var cookwareType in cookwareTypeList)
        {
            var dict = new Dictionary<int, CookingStation>();
            cookingStations.Add(cookwareType, dict);
        }
    }


    public void BakeNavMesh()
    {
        var excludeLayer = LayerMask.NameToLayer("InGameText");
        surface2D.layerMask = ~(1 << excludeLayer);
        surface2D.BuildNavMesh();
    }


    public void AddCookingStation(CookwareType cookwareType, int num)
    {
        var pivot = objectPivotManager.GetCookwarePivots(cookwareType)[num - 1];
        string assetId = string.Format(Strings.cookwareFormat,(int)gameManager.CurrentTheme, cookwareType);
        var obj = Addressables.InstantiateAsync(assetId, pivot).WaitForCompletion();
        OnCookingStationInstantiated(obj, cookwareType, num - 1);
    }

    public void UpgradeCookingStation(InteriorData data, int level)
    {
        var table = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var cookwareData = table.GetData(data.InteriorID);
        var interactionSpeed = 1 - data.EffectQuantity * (level - 1) / 100f;

        var sprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + level).WaitForCompletion();

        var cookingstation = cookingStations[cookwareData.CookwareType][cookwareData.CookwareNB - 1];
        cookingstation.SetInteractionSpeed(interactionSpeed);
        cookingstation.ChangeSpirte(sprite);
    }

    public void OnCookingStationInstantiated(GameObject obj, CookwareType cookwareType,
        int num)
    {
        var cookingStation = obj.GetComponent<CookingStation>();
        cookingStation.transform.InitializeLocalTransform();
        cookingStation.SetId(num);
        cookingStation.cookwareType = cookwareType;
        workFlowController.AddCookingStation(cookingStation);
        cookingStations[cookwareType].Add(num, cookingStation);

        UpdateNavMesh();
    }

    public void AddTable(int num)
    {
        var obj = Addressables.InstantiateAsync(Strings.Table).WaitForCompletion();
        OnTableInstantiated(obj, num - 1);
    }

    public void UpgradeTable(InteriorData data, int level)
    {
        var interactionSpeed = 1 - data.EffectQuantity * (level - 1) / 100f;
        var sprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + level).WaitForCompletion();
        var chairSprite = Addressables.LoadAssetAsync<Sprite>(String.Format(ChairIconID, level)).WaitForCompletion();

        var table = tables[data.InteriorID % 10 - 1];
        table.SetInteractionSpeed(interactionSpeed);
        table.SetEattingSpeed(interactionSpeed);
        table.ChangeSpirte(sprite, chairSprite);
    }

    private void OnTableInstantiated(GameObject obj, int num)
    {
        var table = obj.GetComponent<Table>();
        table.transform.SetParentAndInitialize(tablePivots[num]);
        table.SetId(num);
        workFlowController.AddTable(table);
        tables.Add(num, table);

        UpdateNavMesh();
    }

    public void AddCounter()
    {
        var counterPivot = objectPivotManager.GetCounterPivot();
        var handle = Addressables.InstantiateAsync(Strings.CounterName);
        handle.WaitForCompletion();
        counter = handle.Result.GetComponent<CashierCounter>();
        counter.transform.SetParentAndInitialize(counterPivot);
        workFlowController.SetCashierCounter(counter);
        
        UpdateNavMesh();
    }

    public void UpgradeCounter(InteriorData data, int level)
    {
        var sprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + level).WaitForCompletion();
        counter.ChangeSpirte(sprite);
    }

    public void AddSinkingStation()
    {
        var sinkingStationPivot = objectPivotManager.GetSinkPivot();
        var handle = Addressables.InstantiateAsync(Strings.SinkingStation);
        handle.WaitForCompletion();
        var station = handle.Result.GetComponent<SinkingStation>();
        station.transform.SetParentAndInitialize(sinkingStationPivot);
        workFlowController.SetSinkingStation(station);
        sinkingStation = station;
    }

    public void UpgradeSinkingStation(InteriorData data, int level)
    {
        var capacity = Constants.DEFAULT_SINKINGSTATION_CAPACITY + data.EffectQuantity * (level - 1);
        var sprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + level).WaitForCompletion();
        sinkingStation.ChangeSpirte(sprite);
        sinkingStation.ChangeCapacity(capacity);
    }

    public void AddTrashCan(InteriorData data)
    {
        var pivot = objectPivotManager.GetTrashCanPivot(data.CookwareType);
        var handle = Addressables.InstantiateAsync(Strings.TrashCan);
        handle.WaitForCompletion();
        
        var trashCan = handle.Result.GetComponent<TrashCan>();
        trashCan.transform.SetParentAndInitialize(pivot);
        workFlowController.RegisterTrashCan(data.CookwareType, trashCan);
        trashCans.Add(data.CookwareType, trashCan);
    }

    public void UpgradeTrashCan(InteriorData data, int level)
    {
        var sprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + level).WaitForCompletion();
        trashCans[data.CookwareType].ChangeSpirte(sprite);
    }


    private void UpdateNavMesh()
    {
        surface2D.UpdateNavMesh(surface2D.navMeshData);
    }
}