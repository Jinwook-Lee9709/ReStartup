using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkCooking : InteractWorkBase
{
    private MainLoopWorkContext context;
    private FoodObject foodObject;
    private Sprite foodSprite;

    private ITransportable transformer;

    public WorkCooking(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
        interactTime = workManager.workDurationRatio.WorkDurationRatio[GetType().Name];
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }
    
    public override void OnWorkRegistered()
    {
        base.OnWorkRegistered();

        var iconHandle = Addressables.LoadAssetAsync<Sprite>(context.Consumer.needFood.IconID);
        var backgroundHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Bubble);

        iconHandle.WaitForCompletion();
        backgroundHandle.WaitForCompletion();
        
        Sprite iconSprite = iconHandle.Result;
        Sprite backgroundSprite = backgroundHandle.Result;
        
        var table = target as CookingStation;
        table.ShowIcon(IconPivots.Consumer, iconSprite, backgroundSprite, true);
    }
    
    protected override void HandlePostInteraction()
    {
        var workFlowController = context.WorkFlowController;

        if (!HandleIfCounterUnavailable(workFlowController))
            return;
        
        var station = target as CookingStation;
        station.HideIcon();
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
        var work = new WorkFoodToHall(workManager, WorkType.Kitchen);
        work.SetContext(context, emptyFoodCounter);
        work.SetInteractable(emptyFoodCounter);
        emptyFoodCounter.SetWork(work);
        worker.AssignWork(work);
        nextWork = work;
        nextWorker = worker;
        
        workManager.RegisterConsumerWork(context.Consumer, work);

        transformer = worker as ITransportable;
        foodObject = ServiceLocator.Instance.GetSceneService<GameManager>().ObjectPoolManager
            .GetObjectFromPool<FoodObject>();

        var handle = Addressables.LoadAssetAsync<Sprite>(context.Consumer.needFood.IconID);
        handle.WaitForCompletion();
        OnSpriteLoaded(handle);
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
        station.HideIcon();
        context.WorkFlowController.ReturnCookingStation(station);
        context.Consumer.currentTable.HideIcon();

        base.OnWorkCanceled();
    }
}