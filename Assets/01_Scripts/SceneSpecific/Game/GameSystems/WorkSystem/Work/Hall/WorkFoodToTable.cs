public class WorkFoodToTable : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkFoodToTable(WorkManager workManager, WorkType workType, float interactionTime = 0) : base(workManager, workType, interactionTime)
    {
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }

    protected override void HandlePostInteraction()
    {
        var porter = worker as ITransportable;
        porter.DropPackage(context.Consumer.currentTable.FoodPlacePivot);
        context.Consumer.FSM.OnGetFood();
    }

    public override void OnWorkCanceled()
    {
    }
}