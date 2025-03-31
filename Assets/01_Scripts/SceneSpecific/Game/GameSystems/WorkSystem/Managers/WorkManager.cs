using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkManager : MonoBehaviour
{
    [SerializeField] private WorkerManager workerManager;

    private Dictionary<WorkType, List<WorkBase>> assignedWorks;
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> stoppedWorkQueues;
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> workQueues;

    private void Awake()
    {
        workQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        stoppedWorkQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        assignedWorks = new Dictionary<WorkType, List<WorkBase>>();

        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType)))
            workQueues[taskType] = new PriorityQueue<WorkBase, float>();
        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType)))
            stoppedWorkQueues[taskType] = new PriorityQueue<WorkBase, float>();
        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType))) assignedWorks[taskType] = new List<WorkBase>();
    }

    private void Start()
    {
        workerManager.OnWorkerFree += OnWorkerReturned;
    }

    public void AddWork(WorkBase work)
    {
        var isAssigned = workerManager.AssignWork(work);
        if (isAssigned)
            assignedWorks[work.workType].Add(work);
        else
            workQueues[work.workType].Enqueue(work, Time.time);
    }

    public void AddAssignedWork(WorkBase work)
    {
        assignedWorks[work.workType].Add(work);
    }

    public void AddStoppedWork(WorkType type, WorkBase work)
    {
        stoppedWorkQueues[type].Enqueue(work, Time.time);
    }

    public void OnWorkCanceled(Consumer consumer)
    {
        
    }

    private void OnWorkerReturned(WorkType type)
    {
        if (stoppedWorkQueues[type].Count > 0 && workerManager.IsWorkerAvailable(type))
        {
            var work = stoppedWorkQueues[type].Dequeue();
            workerManager.AssignWork(work);
            assignedWorks[work.workType].Add(work);
            return;
        }

        if (workQueues[type].Count > 0 && workerManager.IsWorkerAvailable(type))
        {
            var work = workQueues[type].Dequeue();
            workerManager.AssignWork(work);
            assignedWorks[work.workType].Add(work);
        }
    }

    public void OnWorkFinished(WorkBase work)
    {
        if (assignedWorks[work.workType].Contains(work))
            assignedWorks[work.workType].Remove(work);
        if (work.NextWork != null)
        {
            var nextWork = work.NextWork;
            if (work.NextWorker == null)
                AddWork(nextWork);
        }
    }
}