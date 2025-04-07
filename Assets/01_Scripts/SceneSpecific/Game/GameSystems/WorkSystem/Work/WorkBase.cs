using System;
using UnityEngine;

public abstract class WorkBase : IComparable<WorkBase>
{
    protected float createdTime = 0f;
    
    public readonly WorkType workType;
    protected WorkerBase worker;
    protected WorkBase nextWork;
    protected WorkerBase nextWorker;
    protected bool isStoppable;
    protected bool isComplete;

    protected WorkManager workManager;
    
    public float CreatedTime => createdTime;
    public bool IsComplete => isComplete;

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
        const float epsilon = 0.0001f; // 허용 오차 값을 정의
        if (Mathf.Abs(CreatedTime - other.CreatedTime) < epsilon)
            return 0; // 두 값이 거의 같으면 동일한 것으로 평가
        return CreatedTime.CompareTo(other.CreatedTime);
    }
}