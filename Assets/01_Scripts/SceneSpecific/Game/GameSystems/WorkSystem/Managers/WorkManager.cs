using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkManager
{
    public WorkDurationRatioSO workDurationRatio;
    private WorkerManager workerManager;
    private Alarm alarm;
    private Dictionary<WorkType, List<WorkBase>> assignedWorks;
    private Dictionary<WorkType, SortedSet<WorkBase>> stoppedWorkQueues;
    private Dictionary<WorkType, SortedSet<WorkBase>> workQueues;
    private List<KeyValuePair<Consumer, WorkBase>> consumerWorkList;
    private HashSet<WorkBase> needHallAlarmWorks = new();
    private HashSet<WorkBase> needKitchenAlarmWorks = new();
    public void Init(WorkerManager workerManager, Alarm alarm)
    {
        SetWorkerManager(workerManager);
        SetAlarm(alarm);
        InitContainers();
        workDurationRatio = Addressables.LoadAssetAsync<WorkDurationRatioSO>(Strings.WorkDurationRatioSO).WaitForCompletion();
    }

    private void SetWorkerManager(WorkerManager workerManager)
    {
        this.workerManager = workerManager;
        workerManager.OnWorkerFree += OnWorkerReturned;
    }
    private void SetAlarm(Alarm alarm)
    {
        this.alarm = alarm;
    }
    private void InitContainers()
    {
        workQueues = new();
        stoppedWorkQueues = new();
        assignedWorks = new Dictionary<WorkType, List<WorkBase>>();
        consumerWorkList = new List<KeyValuePair<Consumer, WorkBase>>();

        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType)))
            workQueues[taskType] = new();
        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType)))
            stoppedWorkQueues[taskType] = new();
        foreach (WorkType taskType in Enum.GetValues(typeof(WorkType))) assignedWorks[taskType] = new List<WorkBase>();
    }

    public void AddWork(WorkBase work, Consumer consumer = null)
    {
        work.OnWorkRegistered();
        var isAssigned = workerManager.AssignWork(work);
        if (isAssigned)
            assignedWorks[work.workType].Add(work);
        else
            workQueues[work.workType].Add(work);
        if (consumer != null)
            consumerWorkList.Add(new KeyValuePair<Consumer, WorkBase>(consumer, work));
    }

    public void RegisterConsumerWork(Consumer consumer, WorkBase work)
    {
        consumerWorkList.Add(new KeyValuePair<Consumer, WorkBase>(consumer, work));
    }

    public void AddStoppedWork(WorkType type, WorkBase work)
    {
        assignedWorks[type].Remove(work);
        work.ResetRegisteredTime();
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
        workQueues.Values.ToList().ForEach(x => x.RemoveWhere(y => workList.Contains(y)));
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
            switch (work.workType)
            {
                case WorkType.Payment:
                case WorkType.Hall:
                    RemoveAlarmWorks(needHallAlarmWorks, work);
                    break;
                case WorkType.Kitchen:
                    RemoveAlarmWorks(needKitchenAlarmWorks, work);
                    break;
            }
            return;
        }

        if (workQueues[type].Count > 0 && workerManager.IsWorkerAvailable(type))
        {
            var work = workQueues[type].Min();
            workQueues[type].Remove(work);
            workerManager.AssignWork(work);
            assignedWorks[work.workType].Add(work);
            switch (work.workType)
            {
                case WorkType.Payment:
                case WorkType.Hall:
                    RemoveAlarmWorks(needHallAlarmWorks, work);
                    break;
                case WorkType.Kitchen:
                    RemoveAlarmWorks(needKitchenAlarmWorks, work);
                    break;
            }
        }
    }

    public void OnWorkFinished(WorkBase work)
    {
        assignedWorks[work.workType].Remove(work);
        switch (work.workType)
        {
            case WorkType.Payment:
            case WorkType.Hall:
                RemoveAlarmWorks(needHallAlarmWorks, work);
                break;
            case WorkType.Kitchen:
                RemoveAlarmWorks(needKitchenAlarmWorks, work);
                break;
        }
        consumerWorkList.RemoveAll(x => x.Value == work);

        if (work.NextWork != null)
        {
            var nextWork = work.NextWork;
            assignedWorks[nextWork.workType].Add(nextWork);
            if (work.NextWorker == null)
                AddWork(nextWork);
        }
    }

    public void UpdateWorkManager(float deltaTime)
    {
        UpdateForAlarm(stoppedWorkQueues, deltaTime);
        UpdateForAlarm(workQueues, deltaTime);
        alarm.UpdateAlarm(needHallAlarmWorks, needKitchenAlarmWorks);
    }

    private void UpdateForAlarm(Dictionary<WorkType, SortedSet<WorkBase>> workQueues, float deltaTime)
    {
        foreach (var workSet in workQueues.Values)
        {
            foreach (var work in workSet)
            {
                work.registeredTime += deltaTime;

                if (work.registeredTime > 2f)
                {
                    switch (work.workType)
                    {
                        case WorkType.Payment:
                        case WorkType.Hall:
                            AddAlarmWorks(needHallAlarmWorks, work);
                            break;
                        case WorkType.Kitchen:
                            AddAlarmWorks(needKitchenAlarmWorks, work);
                            break;
                    }
                }
            }
        }
    }

    private void RemoveAlarmWorks(HashSet<WorkBase> needAlarmWorks, WorkBase work)
    {
        needAlarmWorks.Remove(work);
    }
    private void AddAlarmWorks(HashSet<WorkBase> needAlarmWorks, WorkBase work)
    {
        needAlarmWorks.Add(work);
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
        stoppedWorkQueues[workType].Remove(work);
    }


    #endregion
}