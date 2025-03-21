using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkCooking : WorkBase
{
        
        
    public WorkCooking(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }
    
    private void Init()
    {
        //TODO:asf
        //asd
    }

    public override void OnWorkStopped()
    {
        throw new System.NotImplementedException();
    }

    public override void OnWorkFinished()
    {
        throw new System.NotImplementedException();
    }
}
