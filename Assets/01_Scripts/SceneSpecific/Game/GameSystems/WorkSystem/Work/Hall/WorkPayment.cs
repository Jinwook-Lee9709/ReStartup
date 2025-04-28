using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkPayment : InteractWorkBase
{
    private MainLoopWorkContext context;

    public WorkPayment(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
        interactTime = workManager.workDurationRatio.WorkDurationRatio[GetType().Name];
    }

    public override void OnWorkRegistered()
    {
        base.OnWorkRegistered();

        var iconHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Cash);
        var backgroundHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Bubble);

        iconHandle.WaitForCompletion();
        backgroundHandle.WaitForCompletion();

        var iconSprite = iconHandle.Result;
        var backgroundSprite = backgroundHandle.Result;

        var counter = target as CashierCounter;
        counter.ShowIcon(IconPivots.Consumer, iconSprite, backgroundSprite, true);
    }


    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }

    protected override void StartInteraction()
    {
        base.StartInteraction();
        #region test

        if (!context.Consumer.FSM.alreadyTip)
        {
            context.Consumer.FSM.alreadyTip = !context.Consumer.FSM.alreadyTip;
            context.Consumer.FSM.ConsumerScriptActive(string.Format(Strings.paidverygoodTextFormat, UnityEngine.Random.Range(0, 2), context.Consumer.FSM.consumerData.GuestId));
        }
        #endregion
        if (!context.Consumer.FSM.alreadyTip)
            context.Consumer.FSM.isTip = context.Consumer.FSM.IsTip();

    }

    protected override void HandlePostInteraction()
    {
        var counter = target as CashierCounter;
        counter.HideIcon();
        worker.ClearWork();
        context.WorkFlowController.OnCashierFinished();
    }
}