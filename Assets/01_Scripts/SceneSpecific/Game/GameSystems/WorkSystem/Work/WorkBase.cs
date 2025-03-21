using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WorkBase
{
    public readonly WorkType workType;
    protected WorkManager workManager;
 
     public WorkBase(WorkManager workManager, WorkType workType)
     {
         this.workManager = workManager;
         this.workType = workType;
     }
    
     public abstract void OnAssignWorker(WorkerBase worker);
     public abstract void DoWork();
     public abstract void OnWorkStopped();
     public abstract void OnWorkFinished();
     
}
