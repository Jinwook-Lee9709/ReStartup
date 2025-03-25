using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkFoodToHall : InteractWorkBase
{
    private MainLoopWorkContext context;
    private FoodPickupCounter counter;
    
    public WorkFoodToHall(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }

    public void SetContext(MainLoopWorkContext context, FoodPickupCounter counter)
    {
        this.context = context;
        this.counter = counter;
    }
    protected override void HandlePostInteraction()
    {
        WorkGotoFoodPickupCounter work = new WorkGotoFoodPickupCounter(workManager, WorkType.Hall);

        var transporter = worker as ITransportable;
        FoodPickupCounter counter = target as FoodPickupCounter;
        work.SetContext(context);
        work.SetInteractable(counter);
        transporter.DropPackage(counter.FoodPlacePivot);
        counter.ClearWork();
        counter.SetWork(work);
        nextWork = work;
    }
}