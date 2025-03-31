public class WorkFoodToHall : InteractWorkBase
{
    private MainLoopWorkContext context;
    private FoodPickupCounter counter;

    public WorkFoodToHall(WorkManager workManager, WorkType workType, float interactionTime = 0) : base(workManager, workType, interactionTime)
    {
    }

    public void SetContext(MainLoopWorkContext context, FoodPickupCounter counter)
    {
        this.context = context;
        this.counter = counter;
    }

    protected override void HandlePostInteraction()
    {
        var work = new WorkGotoFoodPickupCounter(workManager, WorkType.Hall);

        var transporter = worker as ITransportable;
        var counter = target as FoodPickupCounter;
        work.SetContext(context);
        work.SetInteractable(counter);
        transporter.DropPackage(counter.FoodPlacePivot);
        counter.SetWork(work);
        nextWork = work;
    }

    public override void OnWorkCanceled()
    {
        base.OnWorkCanceled();
        context.WorkFlowController.ReturnFoodPickupCounter(counter);
        counter.ClearWork();
        
    }
}