using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WorkBase
{
    public static readonly WorkType workType;
    protected WorkManager workManager;
 
     public WorkBase(WorkManager workManager, WorkType workType)
     {
         this.workManager = workManager;
     }
 
     public abstract void OnWorkStopped();
     public abstract void OnWorkFinished();
 }
