using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkCooking : InteractWorkBase
{
    private MainLoopWorkContext context;
    
    private ITransportable transformer;
    private FoodObject foodObject;
    private Sprite foodSprite;
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
        workFlowController.ReturnCookingStation(target as CookingStation);
        
        SetNextWork(workFlowController);
    }
    
    private bool HandleIfCounterUnavailable(WorkFlowController workFlowController)
    {
        if (!workFlowController.IsFoodCounterAvailable())
        {
            workFlowController.RegisterFoodToHall(context, target as CookingStation);
            return false; // 더 이상 진행할 필요 없음
        }
        return true;
    }

    private void SetNextWork(WorkFlowController workFlowController)
    {
        worker.ClearWork();
        var emptyFoodCounter = workFlowController.GetEmptyFoodCounter();
        WorkFoodToHall work = new WorkFoodToHall(workManager, WorkType.Kitchen);
        work.SetContext(context, emptyFoodCounter);
        work.SetInteractable(emptyFoodCounter);
        emptyFoodCounter.SetWork(work);
        worker.AssignWork(work);
        nextWork = work;
        nextWorker = worker;
        
        transformer = worker as ITransportable;
        Addressables.InstantiateAsync("FoodObject").Completed += OnFoodObjectInstantiated;
        Addressables.LoadAssetAsync<Sprite>(context.Consumer.needFood.IconID).Completed += OnSpriteLoaded;
    }
    

    private void OnFoodObjectInstantiated(AsyncOperationHandle<GameObject> handle)
    {
        var obj = handle.Result;
        foodObject = obj.GetComponent<FoodObject>();
        if (foodSprite != null)
        {
            foodObject.SetSprite(foodSprite);
            transformer.LiftPackage(obj);
        }

    }

    private void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle)
    {
        foodSprite = handle.Result;
        if (foodObject != null)
        {
            foodObject.SetSprite(foodSprite);
            transformer.LiftPackage(foodObject.gameObject);
        }
    }

    
    
}