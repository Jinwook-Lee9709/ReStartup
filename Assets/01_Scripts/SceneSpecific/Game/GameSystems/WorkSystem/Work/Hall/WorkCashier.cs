using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkCashier : InteractWorkBase
{
    private MainLoopWorkContext context;
    
    public WorkCashier(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }
    
    
    protected override void HandlePostInteraction()
    {
        var counter = target as CashierCounter;
        worker.ClearWork();
        context.WorkFlowController.OnCashierFinished();
    }
}
