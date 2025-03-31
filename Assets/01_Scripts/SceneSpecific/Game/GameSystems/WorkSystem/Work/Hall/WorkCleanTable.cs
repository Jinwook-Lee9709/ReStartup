public class WorkCleanTable : InteractWorkBase
{
    private WorkFlowController controller;
    private FoodObject foodObject;

    public WorkCleanTable(WorkManager workManager, WorkType workType, float interactionTime = 1) : base(workManager, workType, interactionTime)
    {
    }

    public void SetContext(WorkFlowController controller)
    {
        this.controller = controller;
    }

    public void SetFood(FoodObject foodObject)
    {
        this.foodObject = foodObject;
    }

    protected override void HandlePostInteraction()
    {
        var table = target as Table;
        table.OnCleaned();
        controller.ReturnTable(table);
    }
}