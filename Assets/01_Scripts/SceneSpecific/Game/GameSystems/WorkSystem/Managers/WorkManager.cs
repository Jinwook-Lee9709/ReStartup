using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkManager : MonoBehaviour
{
    [SerializeField] private WorkerManager workerManager;

    private Dictionary<WorkType, List<WorkBase>> assignedWorks;
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> stoppedWorkQueues;
    private Dictionary<WorkType, PriorityQueue<WorkBase, float>> workQueues;
    private List<KeyValuePair<Consumer, WorkBase>> consumerWorkList;

    private void Awake()
    {
        workQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        stoppedWorkQueues = new Dictionary<WorkType, PriorityQueue<WorkBase, float>>();
        assignedWorks = new Dictionary<WorkType, List<WorkBase>>();
        consumerWorkList = new List<KeyValuePair<Consumer, WorkBase>>();

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

    public void AddWork(WorkBase work, Consumer consumer = null)
    {
        var isAssigned = workerManager.AssignWork(work);
        if (isAssigned)
            assignedWorks[work.workType].Add(work);
        else
            workQueues[work.workType].Enqueue(work, Time.time);
        if(consumer is not null)
            consumerWorkList.Add(new KeyValuePair<Consumer, WorkBase>(consumer, work));
    }

    public void RegisterConsumerWork(Consumer consumer, WorkBase work)
    {
        consumerWorkList.Add(new KeyValuePair<Consumer, WorkBase>(consumer, work));
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
        consumerWorkList
            .Where(x => x.Key == consumer)
            .ToList()
            .ForEach(x => x.Value.OnWorkCanceled());
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
        if (consumerWorkList.Any(x => x.Value == work))
        {
            consumerWorkList.RemoveAll(x => x.Value == work);
        }
        if (work.NextWork != null)
        {
            var nextWork = work.NextWork;
            if (work.NextWorker == null)
                AddWork(nextWork);
        }
    }

    #region PlayerLogic

    public void OnPlayerStartWork(WorkBase work, Player player)
    {
        if (work.Worker is null)
        {
            AdjustQueue(work);
        }
        else
        {
            work.Worker.OnWorkFinished();
        }
        player.AssignWork(work);
    }

    private void AdjustQueue(WorkBase work)
    {
        var workType = work.workType;
        workQueues[workType].TryRemove((x) => (x == work), out _);
    }
    

    #endregion
}