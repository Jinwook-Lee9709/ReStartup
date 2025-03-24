using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorkGetOrder : InteractWorkBase
{
    private WorkFlowController workFlowController;

    public void SetWorkFlowController(WorkFlowController workFlowController)
    {
        this.workFlowController = workFlowController;
    }

    public WorkGetOrder(WorkManager workManager, WorkType workType, float interactTime)
        : base(workManager, workType, interactTime)
    {
    }

    protected override void HandlePostInteraction()
    {
        
    }
}