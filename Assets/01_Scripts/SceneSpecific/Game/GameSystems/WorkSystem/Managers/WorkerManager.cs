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
    [SerializeField] private Player player;
    
    //Containers
    private Dictionary<WorkType, List<WorkerBase>> workers;
    private List<WorkerBase> workingWorkers;
    private WorkManager workManager;

    private void Awake()
    {
        InitContaineres();
        GameObject.FindWithTag(Strings.PlayerTag);
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

    //WorkManager 에 Work가 추가됐을때
    //Worker가 작업이 끝났을때
    //화구매니저가 WorkManager한테 -> 빈공간이 있으니, Work를 큐에 추가해라
    //

    //화구매니저 빈공간 있음 -> 일을 등록 -> WorkManager -> 지금 할 수 있는애가 없어 -> Worker가 작업이 끝났을떄 다시 물어봐
    //화구매니저 -> 빈공간 생김 -> 일을 등록 -> WorkManager -> 지금 할 수 있는애가 없어 -> Worker가 작업이 끝났을떄 다시 물어봐
}