using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorkCleanTable : InteractWorkBase
{
    private WorkFlowController controller;
    private bool isPair;

    public WorkCleanTable(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
        interactTime = workManager.workDurationRatio.WorkDurationRatio[GetType().Name];
    }

    public void SetContext(WorkFlowController controller, bool isPair = false)
    {
        this.controller = controller;
        this.isPair = isPair;
    }
    
    public override void OnWorkRegistered()
    {
        base.OnWorkRegistered();

        var iconHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Clean);
        var backgroundHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Bubble);

        iconHandle.WaitForCompletion();
        backgroundHandle.WaitForCompletion();
        
        Sprite iconSprite = iconHandle.Result;
        Sprite backgroundSprite = backgroundHandle.Result;
        
        var table = target as Table;
        table.ShowIcon(IconPivots.Default, iconSprite, backgroundSprite, true);
        table.PairTable.HideIcon();
    }
    

    protected override void HandlePostInteraction()
    {
        var table = target as Table;
        table.HideIcon();
        SetNextWork(table);
        controller.ReturnTable(table);
    }
    
    private void SetNextWork(Table table)
    {
        worker.ClearWork();
        var trayReturnCounter = controller.TrayReturnCounter;
        var work = new WorkDishToKitchen(workManager, WorkType.Hall, 0, false, false);
        int trayCount = isPair ? 2 : 1;
        work.SetContext(controller, trayCount);
        work.SetInteractable(trayReturnCounter);
        worker.AssignWork(work);
        nextWork = work;
        nextWorker = worker;
        
        var transformer = worker as ITransportable;
        var foodObject = table.GetFood();
        transformer.LiftPackage(foodObject);
        if (isPair)
        {
            var subFoodObject = table.PairTable.GetFood();
            subFoodObject.GetComponent<FoodObject>().Release();
        }
    }
    
}