using System;
using UnityEngine;
using UnityEngine.AI;

public class WorkerBase : MonoBehaviour, IComparable<WorkerBase>
{ 
    public enum WorkerState
    {
        Idle,
        ReturnidleArea,
        Working
    }

    private int id;
    
    [SerializeField] protected WorkType workType;
    
    protected NavMeshAgent agent;
    protected WorkBase currentWork;
    protected Transform idleArea;
    protected WorkerManager workerManager;

    public bool IsBusy => currentWork != null;
    public WorkBase CurrentWork => currentWork;
    public WorkType WorkType => workType;
    public bool IsExhausted { get; protected set; }

    
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void Init(WorkerManager manager, Transform idleArea, WorkType workType, int id = 0)
    {
        workerManager = manager;
        this.idleArea = idleArea;
        this.workType = workType;
        this.id = id;
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
        if (IsExhausted)
        {
            workerManager.ReturnExhaustedWorker(this);
            return;
        }
        workerManager.ReturnWorker(this);
    }

    public virtual void OnWorkerExhausted()
    {
        IsExhausted = true;
        if (currentWork is { IsStoppable: false })
            return;
        if (currentWork != null)
        { 
            currentWork.OnWorkStopped();
            currentWork = null;
        }
        workerManager.ReturnExhaustedWorker(this);
    }

    public int CompareTo(WorkerBase other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var idComparison = id.CompareTo(other.id);
        if (idComparison != 0) return idComparison;
        return workType.CompareTo(other.workType);
    }
}