using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPivotManager
{
    public static readonly string PivotFormat = "Pivot{0}";
    private PivotLocator pivotLocator;
    private MapPivotLocator mapPivotLocator;

    public void Init(ThemeIds themeId, MapPivotLocator mapPivotLocator)
    {
        this.mapPivotLocator = mapPivotLocator;
        LoadAndInstantiatePivots(themeId);
        AdjustPivots();
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

    public List<Transform> GetPayWaitingPivots()
    {
        return pivotLocator.PayWaitingPivots;
    }

    public Transform GetWallPivot(ObjectArea area)
    {
        if (area == ObjectArea.Hall)
            return mapPivotLocator.hallWall;
        else
            return mapPivotLocator.kitchenWall;
    }

    public Transform GetFloorPivot(ObjectArea area)
    {
        if (area == ObjectArea.Hall)
            return mapPivotLocator.hallFloor;
        else
            return mapPivotLocator.kitchenFloor;
    }
    

    public Transform GetDecorPivot()
    {
        return mapPivotLocator.decor;
    }
    
    
       private void AdjustPivots()
    {
        var leftPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height * 0.5f, 0f)); 
        var rightPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * 0.5f, 0f));
        var worldInterval = rightPos.x - leftPos.x;
        var defaultInterval = pivotLocator.HallRight.position.x - pivotLocator.HallLeft.position.x;
        var adjustMagnification = worldInterval / defaultInterval;

        foreach (var pair in pivotLocator.IdleAreas)
        {
            foreach (var pivot in pair.Value)
            {
                var info = pivot.GetComponent<PivotInfo>();
                if (info is not null)
                {
                    AdjustObject(pivot, info, defaultInterval, adjustMagnification);
                }
            }
        }
        foreach (var pair in pivotLocator.CookWarePivots)
        {
            foreach (var pivot in pair.Value)
            {
                var info = pivot.GetComponent<PivotInfo>();
                if (info is not null)
                {
                    AdjustObject(pivot, info, defaultInterval, adjustMagnification);
                }
            }
        }

        foreach (var pair in pivotLocator.InteriorPivots)
        {
            var info = pair.Value.GetComponent<PivotInfo>();
            if (info is not null)
            {
                AdjustObject(pair.Value, info, defaultInterval, adjustMagnification);
            }
        }

        foreach (var pivot in pivotLocator.TablePivots)
        {
            var info = pivot.GetComponent<PivotInfo>();
            if (info is not null)
            {
                AdjustObject(pivot, info, defaultInterval, adjustMagnification);
            }
        }
        
        foreach (var pivot in pivotLocator.WatingLinePivots)
        {
            var info = pivot.GetComponent<PivotInfo>();
            if (info is not null)
            {
                AdjustObject(pivot, info, defaultInterval, adjustMagnification);
            }
        }
        
        foreach (var pivot in pivotLocator.PayWaitingPivots)
        {
            var info = pivot.GetComponent<PivotInfo>();
            if (info is not null)
            {
                AdjustObject(pivot, info, defaultInterval, adjustMagnification);
            }
        }
        
        var consumerPivotInfo = pivotLocator.ConsumerSpawnPivot.GetComponent<PivotInfo>();
        if (consumerPivotInfo is not null)
        {
            AdjustObject(pivotLocator.ConsumerSpawnPivot, consumerPivotInfo, defaultInterval, adjustMagnification);
        }
        
        var counterPivotInfo = pivotLocator.CounterPivot.GetComponent<PivotInfo>();
        if (counterPivotInfo is not null)
        {
            AdjustObject(pivotLocator.CounterPivot, counterPivotInfo, defaultInterval, adjustMagnification);
        }
        
        var sinkPivotInfo = pivotLocator.SinkPivot.GetComponent<PivotInfo>();
        if (sinkPivotInfo is not null)
        {
            AdjustObject(pivotLocator.SinkPivot, sinkPivotInfo, defaultInterval, adjustMagnification);
        }
        
        
    }

    private void AdjustObject(Transform pivot, PivotInfo info, float defaultInterval, float adjustMagnification)
    {
        switch (info.objectArea)
        {
            case ObjectArea.Hall:
            {
                var originalX = pivot.position.x;
                var pivotInterval = pivotLocator.HallRight.position.x - originalX;
                var pivotMagnification = pivotInterval / defaultInterval;
                var newX = pivotLocator.HallRight.position.x - defaultInterval * adjustMagnification * pivotMagnification;
                
                var newPosition = pivot.position;
                newPosition.x = newX;
                pivot.position = newPosition;
                break;
            }
            case ObjectArea.Kitchen:
            {
                var originalX = pivot.position.x;
                var pivotInterval = originalX - pivotLocator.KitchenLeft.position.x;
                var pivotMagnification = pivotInterval / defaultInterval;
                var newX = pivotLocator.KitchenLeft.position.x + defaultInterval * adjustMagnification * pivotMagnification;
                
                var newPosition = pivot.position;
                newPosition.x = newX;
                pivot.position = newPosition;
                break;
            }
        }
    }
}