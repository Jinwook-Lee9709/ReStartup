using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkFoodToTable : InteractWorkBase
{
    public WorkFoodToTable(WorkManager workManager, WorkType workType, float interactTime) : base(workManager, workType, interactTime)
    {
    }
    
    protected override void HandlePostInteraction()
    {
        throw new System.NotImplementedException();
    }
}
