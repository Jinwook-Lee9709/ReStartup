using System;
using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using TMPro;
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

    public TrayReturnCounter TrayReturnCounter => _trayReturnCounter;
    
    public int CurrentCookwareCount(CookwareType type) =>
        objectPivotManager.GetCookwarePivots(type).Count(x => x.childCount > 0);

    public void Init(WorkFlowController workFlowController, ObjectPivotManager objectPivotManager, NavMeshSurface surface2D)
    {
        this.workFlowController = workFlowController;
        this.objectPivotManager = objectPivotManager;
        this.surface2D = surface2D;
        
        tablePivots = objectPivotManager.GetTablePivots();
    }

    public void BakeNavMesh()
    {
        int excludeLayer = LayerMask.NameToLayer("InGameText");
        surface2D.layerMask = ~(1 << excludeLayer);
        surface2D.BuildNavMesh();
    }
    

    public void AddCookingStation(CookwareType cookwareType, int num)
    {
        var pivot = objectPivotManager.GetCookwarePivots(cookwareType)[num];
        var handle = Addressables.InstantiateAsync(Strings.CookingStation, pivot);
        handle.Completed += (handle) => OnCookingStationInstantiated(handle, cookwareType, num);
    }

    public void OnCookingStationInstantiated(AsyncOperationHandle<GameObject> handle, CookwareType cookwareType, int num)
    {
        var cookingStation = handle.Result.GetComponent<CookingStation>();
        cookingStation.transform.InitializeLocalTransform();
        cookingStation.SetId(num);
        cookingStation.cookwareType = cookwareType;
        cookingStation.GetComponentInChildren<TextMeshPro>().text = cookwareType.ToString();
        workFlowController.AddCookingStation(cookingStation);

        UpdateNavMesh();
    }

    public void AddTable(int num)
    {
        Addressables.InstantiateAsync(Strings.Table).Completed += (handle)=>
        {
            OnTableInstantiated(handle, num);
        };

    }

    private void OnTableInstantiated(AsyncOperationHandle<GameObject> handle, int num)
    {
        var table = handle.Result.GetComponent<Table>();
        table.transform.SetParentAndInitialize(tablePivots[num]);
        table.SetId(num);
        workFlowController.AddTable(table);

        UpdateNavMesh();
    }

    private void UpdateNavMesh()
    {
        surface2D.UpdateNavMesh(surface2D.navMeshData);
    }
}