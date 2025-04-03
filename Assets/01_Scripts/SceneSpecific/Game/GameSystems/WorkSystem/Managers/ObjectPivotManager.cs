using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPivotManager
{
    private PivotLocator pivotLocator;
    public static readonly string PivotFormat = "Pivot{0}";
    
    public void Init(ThemeIds themeId)
    {
        LoadAndInstantiatePivots(themeId);
    }

    private void LoadAndInstantiatePivots(ThemeIds themeId)
    {
        string assetName = string.Format(PivotFormat, themeId.ToString());
        var instantiateHandle = Addressables.InstantiateAsync(assetName);
        instantiateHandle.WaitForCompletion();
        pivotLocator = instantiateHandle.Result.GetComponent<PivotLocator>();
    }

    public Transform GetCounterPivot()
    {
        return pivotLocator.CounterPivot;
    }
    
    public Transform GetConsumerSpawnPoint()
    {
        return pivotLocator.ConsumerSpawnPivot;
    }

    public Transform GetTrayReturnPivot()
    {
        return pivotLocator.TrayReturnCounterPivot;
    }

    public Transform GetSinkPivot()
    {
        return pivotLocator.SinkPivot;
    }
    
    public List<Transform> GetWatingLines()
    {
        return pivotLocator.WatingLinePivots;
    }

    public List<Transform> GetIdleArea(WorkType type)
    {
        return pivotLocator.IdleAreas[type];
    }

    public List<Transform> GetCookwarePivots(CookwareType type)
    {
        return pivotLocator.CookWarePivots[type];
    }

    public List<Transform> GetTablePivots()
    {
        return pivotLocator.TablePivots;
    }

    public List<Transform> GetPickupCounterPivots()
    {
        return pivotLocator.PickupCounterPivots;
    }
    
}
