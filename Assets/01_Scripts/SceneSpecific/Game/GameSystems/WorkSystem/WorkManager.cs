using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;


public class WorkManager : MonoBehaviour
{
    Dictionary<WorkType, PriorityQueue<WorkBase, float>> taskQueues;
    Dictionary<WorkType, PriorityQueue<WorkBase, float>> canceledTaskQueues;
    private void Awake()
    {
        taskQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        canceledTaskQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        
        foreach (WorkType taskType in System.Enum.GetValues(typeof(WorkType)))
        {
            taskQueues[taskType] = new PriorityQueue<WorkBase, float>();
        }
        foreach (WorkType taskType in System.Enum.GetValues(typeof(WorkType)))
        {
            canceledTaskQueues[taskType] = new PriorityQueue<WorkBase, float>();
        }
    }
    
    public void AddWork(WorkType type, WorkBase work)
    {
        taskQueues[type].Enqueue(work, Time.time);
    }

    public void AddCanceledWork(WorkType type, WorkBase work)
    {
        canceledTaskQueues[type].Enqueue(work, Time.time);
    }
    
}
