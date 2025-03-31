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
        base.OnWorkCanceled();
        var table = target as Table;
        context.WorkFlowController.ReturnTable(table);
        table.ClearWork();
        if (worker == null)
            return;
        var transporter = worker as ITransportable;
        var food = transporter.HandPivot.GetChild(0).GetComponent<FoodObject>();
        if(food != null)
            food.Release();
    }
}