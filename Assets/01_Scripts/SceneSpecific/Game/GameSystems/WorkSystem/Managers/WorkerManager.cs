using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkerManager
{
    //References
    private Dictionary<WorkType, List<Transform>> idleArea = new();

    //Prefab
    private Player player;
    private WorkManager workManager;
    
    //Containers
    private Dictionary<WorkType, SortedSet<WorkerBase>> workers;
    private HashSet<WorkerBase> workingWorkers;
    private HashSet<WorkerBase> exhaustedWorkers;
    
    public int CurrentIdleHallWorkerCount => workers[WorkType.Hall].Count;
    public int CurrentIdleKitchenWorkerCount => workers[WorkType.Hall].Count;

    public void Init(WorkManager workManager)
    {
        this.workManager = workManager;
        InitContaineres();
        InitIdleArea();
    }
    
    private void InitContaineres()
    {
        workers = new Dictionary<WorkType, SortedSet<WorkerBase>>();
        foreach (WorkType workType in Enum.GetValues(typeof(WorkType))) workers.Add(workType, new SortedSet<WorkerBase>());
        workingWorkers = new HashSet<WorkerBase>();
        exhaustedWorkers = new HashSet<WorkerBase>();
    }

    private void InitIdleArea()
    {
        ObjectPivotManager pivotManager = ServiceLocator.Instance.GetSceneService<GameManager>().ObjectPivotManager;
        idleArea.Add(WorkType.All, pivotManager.GetIdleArea(WorkType.All));
        idleArea.Add(WorkType.Hall, pivotManager.GetIdleArea(WorkType.Hall));
        idleArea.Add(WorkType.Kitchen, pivotManager.GetIdleArea(WorkType.Kitchen));
        idleArea.Add(WorkType.Payment, pivotManager.GetIdleArea(WorkType.Payment));
    }
    public void Start()
    {
        InitPlayer();
    }

    private void InitPlayer()
    {
        player = GameObject.FindWithTag(Strings.PlayerTag).GetComponent<Player>();
        player.Init(this, null, WorkType.All);
        player.SetWorkManager(workManager);
    }

    //Events
    public event Action<WorkType> OnWorkerFree;

    public void RegisterWorker(WorkerBase worker, WorkType workType, int id)
    {
        int pivotIndex = id % 10 - 1;
        worker.Init(this, idleArea[workType][pivotIndex], workType, id);
        WarpWorker(worker.GetComponent<NavMeshAgent>(),idleArea[workType][pivotIndex].position).Forget();
        workers[workType].Add(worker);
        OnWorkerFree?.Invoke(workType);
    }

    private async UniTask WarpWorker(NavMeshAgent agent, Vector3 position)
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        agent.Warp(position);
    }

    public bool AssignWork(WorkBase work)
    {
        if (workers[work.workType].Count > 0)
        {
            var sortedSet = workers[work.workType];
            int maxHealth = sortedSet.Max(x => x.GetHealth());
            var listByHealth = sortedSet.Where(x => x.GetHealth() == maxHealth).ToList();
            WorkerBase worker = null;
            if (listByHealth.Count() > 1)
            { 
                float minSpeed = listByHealth.Min(x => x.GetInteractSpeed());
                var listBySpeed = sortedSet.Where(x => Mathf.Approximately(x.GetInteractSpeed(), minSpeed)).ToList();
                if (listBySpeed.Count() > 1)
                {
                    worker = sortedSet.OrderBy(x=>x.Id).First();
                }
                else
                {
                    worker = listBySpeed.First();
                }
            }
            else
            {
                worker = listByHealth.First();
            }
            
            worker.AssignWork(work);
            sortedSet.Remove(worker);
            workingWorkers.Add(worker);
            return true;
        }

        return false;
    }

    public void ReturnWorker(WorkerBase worker)
    {
        if (worker.WorkType == WorkType.All)
            return;
        workingWorkers.Remove(worker);
        workers[worker.WorkType].Add(worker);
        OnWorkerFree?.Invoke(worker.WorkType);
    }
    public void ReturnExhaustedWorker(WorkerBase worker)
    {
        foreach (var pair in workers)
        {
            pair.Value.Remove(worker);
        }
        workingWorkers.Remove(worker);
        
        exhaustedWorkers.Add(worker);
    }

    public void ReturnRecoveredWorker(WorkerBase worker)
    {
        exhaustedWorkers.Remove(worker);
        workers[worker.WorkType].Add(worker);
        OnWorkerFree?.Invoke(worker.WorkType);
    }

    public bool IsWorkerAvailable(WorkType workType)
    {
        return workers[workType].Count > 0;
    }

    public int GetIdleWorkerCount(WorkType workType)
    {
        return workers[workType].Count;
    }
    
}