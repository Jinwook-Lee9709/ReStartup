using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkFoodToHall : InteractWorkBase
{
    public WorkFoodToHall(WorkManager workManager, WorkType workType, float interactTime) : base(workManager, workType,
        interactTime)
    {
    }
    
    public override void OnAssignWorker(WorkerBase worker)
    {
        base.OnAssignWorker(worker);
        ITransformable porter = worker as ITransformable;
        //TODO:Worker손에 음식 들려주기
    }


    protected override void HandlePostInteraction()
    {
        ITransformable porter = worker as ITransformable;
        porter.DropPackage();
        //
    }
}