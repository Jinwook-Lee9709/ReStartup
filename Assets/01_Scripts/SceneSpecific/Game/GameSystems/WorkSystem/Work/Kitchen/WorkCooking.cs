using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkCooking : InteractWorkBase
{
    private MainLoopWorkContext context;
    public WorkCooking(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }
    
    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
        interactTime = Constants.defaultOrderTime;
    }
    
    protected override void HandlePostInteraction()
    {
        WorkFlowController workFlowController = context.WorkFlowController;
       
        if (!HandleIfCounterUnavailable(workFlowController))
            return;
        CookingStation station = target as CookingStation;
        station.ClearWork();
        workFlowController.ReturnCookingStation(target as CookingStation);
        
        SetNextWork(workFlowController);
    }
    
    private bool HandleIfCounterUnavailable(WorkFlowController workFlowController)
    {
        if (!workFlowController.IsFoodCounterAvailable())
        {
            workFlowController.RegisterOrder(context);
            return false; // 더 이상 진행할 필요 없음
        }
        return true;
    }

    private void SetNextWork(WorkFlowController workFlowController)
    {
        worker.ClearWork();
        var emptyFoodCounter = workFlowController.GetEmptyFoodCounter();
        WorkFoodToHall work = new WorkFoodToHall(workManager, WorkType.Kitchen);
        work.SetContext(context);
        work.SetInteractable(emptyFoodCounter);
        emptyFoodCounter.SetWork(work);
        worker.AssignWork(work);
        nextWork = new WorkFoodToHall(workManager, WorkType.Kitchen);
        nextWorker = worker;
    }



    
    
}