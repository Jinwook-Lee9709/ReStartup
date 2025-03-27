using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorkGetOrder : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkGetOrder(WorkManager workManager, WorkType workType)
        : base(workManager, workType)
    {
    }
    
    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
        interactTime = Constants.DEFAULT_ORDER_TIME;
    }

    protected override void HandlePostInteraction()
    {
        context.Consumer.FSM.OnOrderComplete();
        context.WorkFlowController.RegisterOrder(context);
    }

}