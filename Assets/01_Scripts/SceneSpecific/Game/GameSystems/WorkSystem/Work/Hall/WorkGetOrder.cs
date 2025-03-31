public class WorkGetOrder : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkGetOrder(WorkManager workManager, WorkType workType, float interactionTime = 1) : base(workManager, workType, interactionTime)
    {
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }

    protected override void HandlePostInteraction()
    {
        context.Consumer.FSM.OnOrderComplete();
        context.WorkFlowController.RegisterOrder(context);
    }
}