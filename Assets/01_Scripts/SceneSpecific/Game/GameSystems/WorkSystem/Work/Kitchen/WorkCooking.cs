using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkCooking : InteractWorkBase
{
    public WorkCooking(WorkManager workManager, WorkType workType, float interactTime) : base(workManager, workType,
        interactTime)
    {
    }

    protected override void HandlePostInteraction()
    {
        worker.ClearWork();
        
        nextWork = new WorkFoodToHall(workManager, WorkType.Kitchen, 0);
        nextWorker = worker;
    }
}