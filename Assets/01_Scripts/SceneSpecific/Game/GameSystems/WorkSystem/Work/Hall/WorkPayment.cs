public class WorkPayment : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkPayment(WorkManager workManager, WorkType workType) : base(workManager, workType)
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