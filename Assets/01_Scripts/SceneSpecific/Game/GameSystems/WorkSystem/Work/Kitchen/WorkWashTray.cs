using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkWashTray : InteractWorkBase
{
    private WorkFlowController controller;
    
    public WorkWashTray(WorkManager workManager, WorkType workType, bool isInteruptible = true, bool isStoppable = true) : base(workManager, workType, isInteruptible: isInteruptible, isStoppable:isStoppable)
    {
        interactTime = workManager.workDurationRatio.WorkDurationRatio[GetType().Name];
    }
    
    public void SetContext(WorkFlowController controller)
    {
        this.controller = controller;
    }
    
    public override void OnWorkRegistered()
    {
        base.OnWorkRegistered();

        var iconHandle = Addressables.LoadAssetAsync<Sprite>(Strings.WashDish);
        var backgroundHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Bubble);

        iconHandle.WaitForCompletion();
        backgroundHandle.WaitForCompletion();
        
        Sprite iconSprite = iconHandle.Result;
        Sprite backgroundSprite = backgroundHandle.Result;
        
        var table = target as SinkingStation;
        table.ShowIcon(IconPivots.Default, iconSprite, backgroundSprite, true);
    }

    
    protected override void HandlePostInteraction()
    {
        var sinkingStation = target as SinkingStation;
        if (sinkingStation.CurrentTrayCount >= sinkingStation.TrayCapacity)
        {
            worker.ClearWork();
            var work = new WorkWashTray(workManager, WorkType.Kitchen);
            work.SetContext(controller);
            work.SetInteractable(sinkingStation);
            sinkingStation.SetWork(work);
            worker.AssignWork(work);

            nextWork = work;
            nextWorker = worker;
        }
        else
        {
            sinkingStation.HideIcon();
        }
    }
}
