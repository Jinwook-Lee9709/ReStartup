using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkPayment : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkPayment(WorkManager workManager, WorkType workType, float interactionTime = 1) : base(workManager, workType, interactionTime)
    {
    }
    
    public override void OnWorkRegistered()
    {
        base.OnWorkRegistered();

        var iconHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Cash);
        var backgroundHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Bubble);

        iconHandle.WaitForCompletion();
        backgroundHandle.WaitForCompletion();
        
        Sprite iconSprite = iconHandle.Result;
        Sprite backgroundSprite = backgroundHandle.Result;
        
        var counter = target as CashierCounter;
        counter.ShowIcon(IconPivots.Consumer, iconSprite, backgroundSprite, false);
    }


    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }

    protected override void HandlePostInteraction()
    {
        var counter = target as CashierCounter;
        counter.HideIcon();
        worker.ClearWork();
        context.WorkFlowController.OnCashierFinished();
    }
}