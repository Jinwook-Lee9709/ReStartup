using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkGotoCookingStation : InteractWorkBase
{
    private MainLoopWorkContext context;
    private FoodPickupCounter counter;
    private ITransportable transformer;
    
    private FoodObject foodObject;
    private Sprite foodSprite;
    public WorkGotoCookingStation(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }
    
    public void SetContext(MainLoopWorkContext context, FoodPickupCounter counter)
    {
        this.context = context;
        this.counter = counter;
    }
    protected override void HandlePostInteraction()
    {
        worker.ClearWork();
        
        CookingStation station = target as CookingStation;
        context.WorkFlowController.ReturnCookingStation(target as CookingStation);
        
        WorkFoodToHall work = new WorkFoodToHall(workManager, WorkType.Kitchen);
        work.SetContext(context, counter);
        work.SetInteractable(counter);
        counter.SetWork(work);
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

    public override void OnWorkCanceled()
    {
        
    }
}
