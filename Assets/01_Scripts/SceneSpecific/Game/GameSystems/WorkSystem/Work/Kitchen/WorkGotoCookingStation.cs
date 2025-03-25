using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkGotoCookingStation : InteractWorkBase
{
    private MainLoopWorkContext context;
    
    public WorkGotoCookingStation(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }
    
    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }
    protected override void HandlePostInteraction()
    {
        worker.ClearWork();
        WorkFoodToHall work = new WorkFoodToHall(workManager, WorkType.Kitchen);
        work.SetContext(context);
        
        nextWork = new WorkFoodToHall(workManager, WorkType.Kitchen);
        nextWorker = worker;
    }
}
