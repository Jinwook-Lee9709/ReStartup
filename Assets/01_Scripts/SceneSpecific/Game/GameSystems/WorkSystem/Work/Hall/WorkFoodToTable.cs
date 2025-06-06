public class WorkFoodToTable : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkFoodToTable(WorkManager workManager, WorkType workType, float interactionTime = 0,
        bool isInteruptable = false, bool isStoppable = false) : base(workManager, workType, interactionTime,
        isInteruptable, isStoppable)
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
        var table = target as Table;
        table.ClearWork();
        table.HideIcon();

        if (worker != null)
        {
            var transporter = worker as ITransportable;
            var food = transporter.HandPivot.GetChild(0).GetComponent<FoodObject>();
            if (food != null)
                food.Release();
        }
        base.OnWorkCanceled();
    
    }
}