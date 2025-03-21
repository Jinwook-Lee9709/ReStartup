using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;


public class WorkManager : MonoBehaviour
{
    [SerializeField] private WorkerManager workerManager;
    
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> taskQueues;
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> canceledWorkQueues;

    private Dictionary<WorkType, List<WorkBase>> assignedWorks;
    private void Awake()
    {
        taskQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        canceledWorkQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        assignedWorks = new Dictionary<WorkType, List<WorkBase>>();
        
        foreach (WorkType taskType in System.Enum.GetValues(typeof(WorkType)))
        {
            taskQueues[taskType] = new PriorityQueue<WorkBase, float>();
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
        Debug.Log(isAssigned);
        if (isAssigned)
        {
            assignedWorks[work.workType].Add(work);
        }
        else
        {
            taskQueues[work.workType].Enqueue(work, Time.time);
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
    
}
