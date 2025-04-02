using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkerManager : MonoBehaviour
{
    //References
    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<WorkType, List<Transform>> idleArea;

    //Prefab
    private Player player;
    [SerializeField] private WorkManager workManager;
    
    //Containers
    private Dictionary<WorkType, SortedSet<WorkerBase>> workers;
    private List<WorkerBase> workingWorkers;
    
    public int CurrentIdleHallWorkerCount => workers[WorkType.Hall].Count;
    public int CurrentIdleKitchenWorkerCount => workers[WorkType.Hall].Count;

    private void Awake()
    {
        InitContaineres();
    }
    private void Start()
    {
        InitPlayer();
    }

    private void InitPlayer()
    {
        player = GameObject.FindWithTag(Strings.PlayerTag).GetComponent<Player>();
        player.Init(this, null, WorkType.All);
        player.SetWorkManager(workManager);
    }

    private void InitContaineres()
    {
        workers = new Dictionary<WorkType, SortedSet<WorkerBase>>();
        foreach (WorkType workType in Enum.GetValues(typeof(WorkType))) workers.Add(workType, new SortedSet<WorkerBase>());
        workingWorkers = new List<WorkerBase>();
    }

    //Events
    public event Action<WorkType> OnWorkerFree;

    public void RegisterWorker(WorkerBase worker, WorkType workType, int id)
    {
        int pivotIndex = id % 10 - 1;
        worker.Init(this, idleArea[workType][pivotIndex], workType, id);
        worker.GetComponent<NavMeshAgent>().Warp(idleArea[workType][pivotIndex].position);
        workers[workType].Add(worker);
        OnWorkerFree?.Invoke(workType);
    }

    public bool AssignWork(WorkBase work)
    {
        if (workers[work.workType].Count > 0)
        {
            var sortedSet = workers[work.workType];
            var worker = sortedSet.Min;
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

    public bool IsWorkerAvailable(WorkType workType)
    {
        return workers[workType].Count > 0;
    }

    public int GetIdleWorkerCount(WorkType workType)
    {
        return workers[workType].Count;
    }
    
}