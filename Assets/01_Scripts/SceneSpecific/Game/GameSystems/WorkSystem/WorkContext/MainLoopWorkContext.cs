public class MainLoopWorkContext
{
    public MainLoopWorkContext(Consumer consumer, WorkFlowController workFlowController)
    {
        this.Consumer = consumer;
        this.WorkFlowController = workFlowController;
    }

    public Consumer Consumer { get; }

    public WorkFlowController WorkFlowController { get; }
}