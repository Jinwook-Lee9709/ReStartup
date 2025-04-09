using System;
using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class WorkStationManager
{
    private ObjectPivotManager objectPivotManager;
    private WorkFlowController workFlowController;
    private List<Transform> tablePivots;
    private NavMeshSurface surface2D;
    private TrayReturnCounter _trayReturnCounter;

    Dictionary<int, Table> tables = new();
    Dictionary<CookwareType, Dictionary<int, CookingStation>> cookingStations = new();
    SinkingStation sinkingStation;

    public TrayReturnCounter TrayReturnCounter => _trayReturnCounter;

    public int CurrentCookwareCount(CookwareType type) =>
        objectPivotManager.GetCookwarePivots(type).Count(x => x.childCount > 0);

    public void Init(WorkFlowController workFlowController, ObjectPivotManager objectPivotManager,
        NavMeshSurface surface2D)
    {
        this.workFlowController = workFlowController;
        this.objectPivotManager = objectPivotManager;
        this.surface2D = surface2D;

        tablePivots = objectPivotManager.GetTablePivots();

        InitContainer();
    }

    private void InitContainer()
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;

        var table = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString());
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
        int excludeLayer = LayerMask.NameToLayer("InGameText");
        surface2D.layerMask = ~(1 << excludeLayer);
        surface2D.BuildNavMesh();
    }


    public void AddCookingStation(CookwareType cookwareType, int num)
    {
        var pivot = objectPivotManager.GetCookwarePivots(cookwareType)[num - 1];
        var obj = Addressables.InstantiateAsync(Strings.CookingStation, pivot).WaitForCompletion();
        OnCookingStationInstantiated(obj, cookwareType, num - 1);
    }

    public void UpgradeCookingStation(InteriorData data, int level)
    {
        var table = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var cookwareData = table.GetData(data.InteriorID);
        float interactionSpeed = (1 - data.EffectQuantity * (level - 1) / 100f);
        Debug.Log(interactionSpeed);
        cookingStations[cookwareData.CookwareType][cookwareData.CookwareNB - 1].SetInteractionSpeed(interactionSpeed);
    }

    public void OnCookingStationInstantiated(GameObject obj, CookwareType cookwareType,
        int num)
    {
        var cookingStation = obj.GetComponent<CookingStation>();
        cookingStation.transform.InitializeLocalTransform();
        cookingStation.SetId(num);
        cookingStation.cookwareType = cookwareType;
        cookingStation.GetComponentInChildren<TextMeshPro>().text = cookwareType.ToString();
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
        float interactionSpeed = (1 - data.EffectQuantity * (level - 1) / 100f);
        tables[data.InteriorID % 10 - 1].SetInteractionSpeed(interactionSpeed);
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
        int capacity = Constants.DEFAULT_SINKINGSTATION_CAPACITY + data.EffectQuantity * (level - 1); 
        sinkingStation.ChangeCapacity(capacity);
    }


    private void UpdateNavMesh()
    {
        surface2D.UpdateNavMesh(surface2D.navMeshData);
    }
}