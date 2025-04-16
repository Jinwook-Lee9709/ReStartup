using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPivotManager
{
    public static readonly string PivotFormat = "Pivot{0}";
    private PivotLocator pivotLocator;

    public void Init(ThemeIds themeId)
    {
        LoadAndInstantiatePivots(themeId);
    }

    private void LoadAndInstantiatePivots(ThemeIds themeId)
    {
        var assetName = string.Format(PivotFormat, themeId.ToString());
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

    public Transform GetInteriorPivot(int id)
    {
        var dictionary = pivotLocator.InteriorPivots;
        dictionary.TryGetValue(id, out var pivot);
        return pivot;
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

    public List<Transform> GetPayWaitingPibots()
    {
        return pivotLocator.PayWaitingPivots;
    }
}