public abstract class WorkBase
{
    public readonly WorkType workType;
    protected WorkBase nextWork;
    protected WorkerBase nextWorker;

    protected WorkManager workManager;

    protected WorkBase(WorkManager workManager, WorkType workType)
    {
        this.workManager = workManager;
        this.workType = workType;
    }
    protected WorkerBase worker;
    public WorkerBase Worker => worker;
    public WorkBase NextWork => nextWork;
    public WorkerBase NextWorker => nextWorker;

    public abstract void OnWorkAssigned();
    public abstract void OnAssignWorker(WorkerBase worker);
    public abstract void DoWork();
    public abstract void OnWorkCanceled();
    public abstract void OnWorkStopped();
    public abstract void OnWorkFinished();

    protected enum WorkPhase
    {
        Moving,
        Working
    }
}