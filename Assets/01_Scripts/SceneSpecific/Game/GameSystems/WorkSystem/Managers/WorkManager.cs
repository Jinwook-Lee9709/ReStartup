using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkManager
{
    private WorkerManager workerManager;
    private Dictionary<WorkType, List<WorkBase>> assignedWorks;
    private Dictionary<WorkType, SortedSet<WorkBase>> stoppedWorkQueues;
    private Dictionary<WorkType, SortedSet<WorkBase>> workQueues;
    private List<KeyValuePair<Consumer, WorkBase>> consumerWorkList;

    public void Init(WorkerManager workerManager)
    {
        SetWorkerManager(workerManager);
        InitContainers();
    }

    private void SetWorkerManager(WorkerManager workerManager)
    {
        this.workerManager = workerManager;
        workerManager.OnWorkerFree += OnWorkerReturned;
    }

    private void InitContainers()
    {
        workQueues = new ();
        stoppedWorkQueues = new ();
        assignedWorks = new Dictionary<WorkType, List<WorkBase>>();
        consumerWorkList = new List<KeyValuePair<Consumer, WorkBase>>();

        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType)))
            workQueues[taskType] = new ();
        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType)))
            stoppedWorkQueues[taskType] = new ();
        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType))) assignedWorks[taskType] = new List<WorkBase>();
    }

    public void AddWork(WorkBase work, Consumer consumer = null)
    {
        var isAssigned = workerManager.AssignWork(work);
        if (isAssigned)
            assignedWorks[work.workType].Add(work);
        else
            workQueues[work.workType].Add(work);
        if(consumer !=null)
            consumerWorkList.Add(new KeyValuePair<Consumer, WorkBase>(consumer, work));
    }

    public void RegisterConsumerWork(Consumer consumer, WorkBase work)
    {
        consumerWorkList.Add(new KeyValuePair<Consumer, WorkBase>(consumer, work));
    }

    public void AddStoppedWork(WorkType type, WorkBase work)
    {
        assignedWorks[type].Remove(work);
        stoppedWorkQueues[type].Add(work);
    }

    public void OnWorkCanceled(Consumer consumer)
    {
        var workList = consumerWorkList
            .Where(x => x.Key == consumer)
            .Select(x => x.Value);
        foreach (var work in workList)
        {
            stoppedWorkQueues[work.workType].Remove(work);
        }
        consumerWorkList
            .Where(x => x.Key == consumer)
            .ToList()
            .ForEach(x => x.Value.OnWorkCanceled());

    }

    private void OnWorkerReturned(WorkType type)
    {
        if (stoppedWorkQueues[type].Count > 0 && workerManager.IsWorkerAvailable(type))
        {
            var work = stoppedWorkQueues[type].Min;
            stoppedWorkQueues[type].Remove(work);
            workerManager.AssignWork(work);
            assignedWorks[work.workType].Add(work);
            return;
        }

        if (workQueues[type].Count > 0 && workerManager.IsWorkerAvailable(type))
        {
            var work = workQueues[type].Min();
            workQueues[type].Remove(work);
            workerManager.AssignWork(work);
            assignedWorks[work.workType].Add(work);
        }
    }

    public void OnWorkFinished(WorkBase work)
    {
        assignedWorks[work.workType].Remove(work);
        
        consumerWorkList.RemoveAll(x => x.Value == work);

        if (work.NextWork != null)
        {
            var nextWork = work.NextWork;
            assignedWorks[nextWork.workType].Add(nextWork);
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
        workQueues[workType].Remove(work);
    }
    

    #endregion
}