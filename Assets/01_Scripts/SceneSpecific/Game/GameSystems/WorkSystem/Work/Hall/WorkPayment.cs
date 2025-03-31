public class WorkPayment : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkPayment(WorkManager workManager, WorkType workType, float interactionTime = 1) : base(workManager, workType, interactionTime)
    {
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }

    protected override void HandlePostInteraction()
    {
        worker.ClearWork();
        context.WorkFlowController.OnCashierFinished();
    }
}