using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkFoodToTable : InteractWorkBase
{
    public WorkFoodToTable(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }
    
    protected override void HandlePostInteraction()
    {
        throw new System.NotImplementedException();
    }
}
