using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;


public class WorkManager : MonoBehaviour
{
    [SerializeField] private WorkerManager workerManager;
    
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> workQueues;
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> canceledWorkQueues;

    private Dictionary<WorkType, List<WorkBase>> assignedWorks;
    private void Awake()
    {
        workQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        canceledWorkQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        assignedWorks = new Dictionary<WorkType, List<WorkBase>>();
        
        foreach (WorkType taskType in System.Enum.GetValues(typeof(WorkType)))
        {
            workQueues[taskType] = new PriorityQueue<WorkBase, float>();
        }
        foreach (WorkType taskType in System.Enum.GetValues(typeof(WorkType)))
        {
            canceledWorkQueues[taskType] = new PriorityQueue<WorkBase, float>();
        }
        foreach (WorkType taskType in System.Enum.GetValues(typeof(WorkType)))
        {
            assignedWorks[taskType] = new List<WorkBase>();
        }
    }

    private void Start()
    {
        workerManager.OnWorkFinished += OnWorkerReturned;
    }
    
    public void AddWork(WorkBase work)
    {
        bool isAssigned = workerManager.AssignWork(work);
        if (isAssigned)
        {
            assignedWorks[work.workType].Add(work);
        }
        else
        {
            workQueues[work.workType].Enqueue(work, Time.time);
        }
    }

    public void AddCanceledWork(WorkType type, WorkBase work)
    {
        canceledWorkQueues[type].Enqueue(work, Time.time);
    }

    private void OnWorkerReturned(WorkType type)
    {
        if (canceledWorkQueues[type].Count > 0 && workerManager.IsWorkerAvailable(type))
        {
            WorkBase work = canceledWorkQueues[type].Peek();
            workerManager.AssignWork(work);
            assignedWorks[work.workType].Add(work);
        }
    }

    public void OnWorkFinished(WorkBase work)
    {
        if(assignedWorks[work.workType].Contains(work))
            assignedWorks[work.workType].Remove(work);
        if (work.NextWork != null)
        {
            var nextWork = work.NextWork;
            if (work.NextWorker == null)
                AddWork(nextWork);

        }
    }
    
}
