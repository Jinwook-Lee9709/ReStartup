using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkWashTray : InteractWorkBase
{
    private WorkFlowController controller;
    
    public WorkWashTray(WorkManager workManager, WorkType workType, float interactTime = 1, bool isInteruptible = true, bool isStoppable = true) : base(workManager, workType, interactTime, isInteruptible, isStoppable)
    {
    }
    
    public void SetContext(WorkFlowController controller)
    {
        this.controller = controller;
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
            worker.AssignWork(work);
            sinkingStation.SetWork(work);
            nextWork = work;
            nextWorker = worker;
        }
        
    }
}
