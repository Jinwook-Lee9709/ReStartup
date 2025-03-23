using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WorkBase
{
    protected enum WorkPhase
    {
        Moving,
        Working
    }
    
    protected WorkManager workManager;
    protected WorkBase nextWork;
    protected WorkerBase nextWorker;
    public readonly WorkType workType;

    public WorkBase NextWork => nextWork;
    public WorkerBase NextWorker => nextWorker;
 
     protected WorkBase(WorkManager workManager, WorkType workType)
     {
         this.workManager = workManager;
         this.workType = workType;
     }
     
     public abstract void OnWorkAssigned();
     public abstract void OnAssignWorker(WorkerBase worker);
     public abstract void DoWork();
     public abstract void OnWorkStopped();
     public abstract void OnWorkFinished();
     
}
