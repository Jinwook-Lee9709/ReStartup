using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkGotoCookingStation : InteractWorkBase
{
    private MainLoopWorkContext context;
    private FoodPickupCounter counter;

    private FoodObject foodObject;
    private Sprite foodSprite;
    private ITransportable transformer;

    public WorkGotoCookingStation(WorkManager workManager, WorkType workType, float interactionTime = 0) : base(workManager, workType, interactionTime)
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

        var station = target as CookingStation;
        context.WorkFlowController.ReturnCookingStation(station);

        var work = new WorkFoodToHall(workManager, WorkType.Kitchen);
        work.SetContext(context, counter);
        work.SetInteractable(counter);
        counter.SetWork(work);
        worker.AssignWork(work);
        nextWork = work;
        nextWorker = worker;
        
        workManager.RegisterConsumerWork(context.Consumer, work);

        transformer = worker as ITransportable;
        foodObject = ServiceLocator.Instance.GetSceneService<GameManager>().ObjectPoolManager
            .GetObjectFromPool<FoodObject>();
        
        Addressables.LoadAssetAsync<Sprite>(context.Consumer.needFood.IconID).Completed += OnSpriteLoaded;
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
        var station = target as CookingStation;
        station.ClearWork();
        context.WorkFlowController.ReturnCookingStation(station);

        base.OnWorkCanceled();
    }
}