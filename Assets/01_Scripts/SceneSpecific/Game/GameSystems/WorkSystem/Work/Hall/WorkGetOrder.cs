using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkGetOrder : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkGetOrder(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
        interactTime = workManager.workDurationRatio.WorkDurationRatio[GetType().Name];
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
        
        var table = target as Table;
        table.ShowIcon(IconPivots.Consumer, iconSprite, backgroundSprite, true);
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }

    protected override void HandlePostInteraction()
    {
        context.Consumer.FSM.OnOrderComplete();
        context.WorkFlowController.RegisterOrder(context);
    }
}