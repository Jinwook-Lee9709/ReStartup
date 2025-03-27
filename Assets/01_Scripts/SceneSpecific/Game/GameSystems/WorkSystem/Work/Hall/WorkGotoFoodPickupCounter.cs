public class WorkGotoFoodPickupCounter : InteractWorkBase
{
    private MainLoopWorkContext context;
    private FoodObject foodObject;

    public WorkGotoFoodPickupCounter(WorkManager workManager, WorkType workType) : base(workManager, workType)
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

        var work = new WorkFoodToTable(workManager, WorkType.Hall);
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
    }
}