public class WorkGotoFoodPickupCounter : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkGotoFoodPickupCounter(WorkManager workManager, WorkType workType, float interactionTime = 0) : base(workManager, workType, interactionTime)
    {
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }

    protected override void HandlePostInteraction()
    {
        worker.ClearWork();

        var counter = target as FoodPickupCounter;
        context.WorkFlowController.ReturnFoodPickupCounter(target as FoodPickupCounter);

        var work = new WorkFoodToTable(workManager, WorkType.Hall, 0);
        work.SetContext(context);
        work.SetInteractable(context.Consumer.currentTable);
        context.Consumer.currentTable.SetWork(work);
        worker.AssignWork(work);

        var transporter = worker as ITransportable;
        var package = counter.LiftFood();
        transporter.LiftPackage(package);

        nextWork = work;
        nextWorker = worker;
    }

    public override void OnWorkCanceled()
    {
        base.OnWorkCanceled();
        var counter = target as FoodPickupCounter;
        context.WorkFlowController.ReturnFoodPickupCounter(counter);
        counter.ClearWork();
        var food = counter.FoodPlacePivot.GetChild(0).GetComponent<FoodObject>();
        if(food != null)
            food.Release();
    }
}