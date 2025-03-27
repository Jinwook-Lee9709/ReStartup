using UnityEngine;
using UnityEngine.AI;

public class WorkerBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected WorkBase currentWork;
    protected Transform idleArea;

    protected WorkerManager workerManager;

    private bool IsBusy => currentWork != null;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void Init(WorkerManager manager, Transform idleArea)
    {
        workerManager = manager;
        this.idleArea = idleArea;
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