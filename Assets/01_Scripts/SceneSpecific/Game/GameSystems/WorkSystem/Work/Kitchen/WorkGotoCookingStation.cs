using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkGotoCookingStation : InteractWorkBase
{
    private MainLoopWorkContext context;
    private FoodPickupCounter counter;
    
    public WorkGotoCookingStation(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }
    
    public void SetContext(MainLoopWorkContext context, FoodPickupCounter counter)
    {
        this.context = context;
        this.counter = counter;
    }
    protected override void HandlePostInteraction()
    {
        worker.ClearWork();
        
        CookingStation station = target as CookingStation;
        station.ClearWork();
        context.WorkFlowController.ReturnCookingStation(target as CookingStation);
        
        WorkFoodToHall work = new WorkFoodToHall(workManager, WorkType.Kitchen);
        work.SetContext(context, counter);
        work.SetInteractable(counter);
        counter.SetWork(work);
        worker.AssignWork(work);
        nextWork = work;
        nextWorker = worker;
    }
}
