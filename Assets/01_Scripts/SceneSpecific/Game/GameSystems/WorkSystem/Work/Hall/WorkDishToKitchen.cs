public class WorkDishToKitchen : InteractWorkBase
{
    private WorkFlowController controller;
    private int trayCount;

    public WorkDishToKitchen(WorkManager workManager, WorkType workType, float interactTime = 1,
        bool isInteruptible = true, bool isStoppable = true) : base(workManager, workType, interactTime, isInteruptible,
        isStoppable)
    {
    }

    public void SetContext(WorkFlowController controller, int trayCount = 1)
    {
        this.controller = controller;
        this.trayCount = trayCount;
    }

    protected override void HandlePostInteraction()
    {
        controller.SinkingStation.AddTray(trayCount);
        var transporter = worker as ITransportable;
        var tray = transporter.HandPivot.GetChild(0).GetComponent<FoodObject>();
        tray.Release();
    }
}