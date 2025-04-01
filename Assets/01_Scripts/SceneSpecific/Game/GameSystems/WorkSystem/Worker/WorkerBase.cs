using UnityEngine;
using UnityEngine.AI;

public class WorkerBase : MonoBehaviour
{ 
    protected WorkType workType;
    
    protected NavMeshAgent agent;
    protected WorkBase currentWork;
    protected Transform idleArea;
    protected WorkerManager workerManager;

    public bool IsBusy => currentWork != null;
    public WorkBase CurrentWork => currentWork;
    public WorkType WorkType => workType;
    
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void Init(WorkerManager manager, Transform idleArea, WorkType workType)
    {
        workerManager = manager;
        this.idleArea = idleArea;
        this.workType = workType;
    }

    public virtual void AssignWork(WorkBase work)
    {
        if (IsBusy)
        {
            Debug.LogWarning($"{gameObject.name} is already busy!");
            return;
        }

        currentWork = work;
        currentWork.OnAssignWorker(this);
    }

    public void ClearWork()
    {
        currentWork = null;
    }

    public virtual void OnWorkFinished()
    {
        currentWork = null;
        workerManager.ReturnWorker(this);
    }

    public virtual void StopWork()
    {
        if (currentWork != null)
        {
            currentWork.OnWorkStopped();
            OnWorkFinished();
        }
    }
}