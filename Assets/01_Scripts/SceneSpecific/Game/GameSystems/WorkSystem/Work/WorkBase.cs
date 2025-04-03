using System;
using UnityEngine;

public abstract class WorkBase : IComparable<WorkBase>
{
    private float createdTime = 0f;
    
    public readonly WorkType workType;
    protected WorkerBase worker;
    protected WorkBase nextWork;
    protected WorkerBase nextWorker;
    protected bool isStoppable;

    protected WorkManager workManager;
    
    public float CreatedTime => createdTime;

    protected WorkBase(WorkManager workManager, WorkType workType, bool isStopable = true)
    {
        createdTime = Time.time;
        this.workManager = workManager;
        this.workType = workType;
        this.isStoppable = isStopable;
    }

    public void ModifyPriority(float priority)
    {
        createdTime = priority;
    }

    public WorkerBase Worker => worker;
    public WorkBase NextWork => nextWork;
    public WorkerBase NextWorker => nextWorker;
    public bool IsStoppable => isStoppable;

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

    public int CompareTo(WorkBase other)
    {
        return CreatedTime.CompareTo(other.CreatedTime);
    }
}