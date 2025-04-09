using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkGetPairOrder : InteractWorkBase
{
    private MainLoopWorkContext firstConsumerContext;
    private MainLoopWorkContext secondConsumerContext;


    public WorkGetPairOrder(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }

    public void SetContext(MainLoopWorkContext first, MainLoopWorkContext second)
    {
        firstConsumerContext = first;
        secondConsumerContext = second;
    }
    
    public override void OnWorkRegistered()
    {
        base.OnWorkRegistered();

        var firstConsumerFoodIcon = Addressables.LoadAssetAsync<Sprite>(firstConsumerContext.Consumer.needFood.IconID);
        var secondConsumerFoodIcon = Addressables.LoadAssetAsync<Sprite>(secondConsumerContext.Consumer.needFood.IconID);
        var backgroundHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Bubble);

        firstConsumerFoodIcon.WaitForCompletion();
        secondConsumerFoodIcon.WaitForCompletion();
        backgroundHandle.WaitForCompletion();
        
        Sprite firstConsumerFoodIconSprite = firstConsumerFoodIcon.Result;
        Sprite secondConsumerFoodIconSprite = firstConsumerFoodIcon.Result;
        Sprite backgroundSprite = backgroundHandle.Result;
        
        var mainTable = target as Table;
        var subTable = mainTable.PairTable; 
        mainTable.ShowIcon(IconPivots.Consumer, firstConsumerFoodIconSprite, backgroundSprite, true);
        subTable.ShowIcon(IconPivots.Consumer, secondConsumerFoodIconSprite, backgroundSprite, false);
    }

    protected override void HandlePostInteraction()
    {
        firstConsumerContext.WorkFlowController.RegisterOrder(firstConsumerContext);
        secondConsumerContext.WorkFlowController.RegisterOrder(secondConsumerContext);
        firstConsumerContext.Consumer.FSM.OnOrderComplete();
        secondConsumerContext.Consumer.FSM.OnOrderComplete();
    }
}
