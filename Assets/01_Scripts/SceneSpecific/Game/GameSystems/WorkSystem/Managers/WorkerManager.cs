using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    
    //References
    private WorkManager workManager;
    
    //Containers
    private Dictionary<WorkType, List<WorkerBase>> workers;
    private Dictionary<WorkType, List<WorkerBase>> workingWorkers;
    
    //Events
    public event Action<WorkType> OnWorkFinished;
    [SerializeField] private WorkerBase testWorker;

    private void Awake()
    {
        workers = new Dictionary<WorkType, List<WorkerBase>>();
        foreach (WorkType workType in Enum.GetValues(typeof(WorkType)))
        {
            workers.Add(workType, new List<WorkerBase>());
        }
        workingWorkers = new Dictionary<WorkType, List<WorkerBase>>();
        foreach (WorkType workType in Enum.GetValues(typeof(WorkType)))
        {
            workingWorkers.Add(workType, new List<WorkerBase>());
        }
    }

    private void Start()
    {
         workers[WorkType.Hall].Add(testWorker);
    }
    public bool AssignWork(WorkBase work)
    {
        if (workers[work.workType].Count > 0)
        {
            var list = workers[work.workType];
            WorkerBase worker = list[0];
            list[0].AssignWork(work);
            list.RemoveAt(0);
            workingWorkers[work.workType].Add(worker);
            return true;
        }

        return false;
        
    }

    public void ReturnWorker(WorkerBase worker)
    {
        foreach (var pair in workingWorkers)
        {
            if (pair.Value.Contains(worker))
            {
                pair.Value.Remove(worker);
                workers[pair.Key].Add(worker);
                return;
            }
        }
        
        Debug.LogError("Worker not found");
    }

    public bool IsWorkerAvailable(WorkType workType)
    {
        return true;
    }
    
    
    //WorkManager 에 Work가 추가됐을때
    //Worker가 작업이 끝났을때
    //화구매니저가 WorkManager한테 -> 빈공간이 있으니, Work를 큐에 추가해라
    //

    //화구매니저 빈공간 있음 -> 일을 등록 -> WorkManager -> 지금 할 수 있는애가 없어 -> Worker가 작업이 끝났을떄 다시 물어봐
    //화구매니저 -> 빈공간 생김 -> 일을 등록 -> WorkManager -> 지금 할 수 있는애가 없어 -> Worker가 작업이 끝났을떄 다시 물어봐




}
