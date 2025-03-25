using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoopWorkContext
{
    private readonly Consumer consumer;
    private readonly WorkFlowController workFlowController;
    
    public Consumer Consumer => consumer;
    public WorkFlowController WorkFlowController => workFlowController;
    public MainLoopWorkContext(Consumer consumer, WorkFlowController workFlowController)
    {
        this.consumer = consumer;
        this.workFlowController = workFlowController;
    }
    

}
