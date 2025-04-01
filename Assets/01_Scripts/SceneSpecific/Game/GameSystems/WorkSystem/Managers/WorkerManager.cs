using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkerManager : MonoBehaviour
{
    //References
    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<WorkType, Transform> idleArea;

    //Prefab
    private Player player;
    [SerializeField] private WorkManager workManager;
    
    //Containers
    private Dictionary<WorkType, List<WorkerBase>> workers;
    private List<WorkerBase> workingWorkers;

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
        workers = new Dictionary<WorkType, List<WorkerBase>>();
        foreach (WorkType workType in Enum.GetValues(typeof(WorkType))) workers.Add(workType, new List<WorkerBase>());
        workingWorkers = new List<WorkerBase>();
    }

    //Events
    public event Action<WorkType> OnWorkerFree;

    public void RegisterWorker(WorkerBase worker, WorkType workType)
    {
        worker.Init(this, idleArea[workType], workType);
        workers[workType].Add(worker);
        OnWorkerFree?.Invoke(workType);
    }

    public bool AssignWork(WorkBase work)
    {
        if (workers[work.workType].Count > 0)
        {
            var list = workers[work.workType];
            var worker = list[0];
            list[0].AssignWork(work);
            list.RemoveAt(0);
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