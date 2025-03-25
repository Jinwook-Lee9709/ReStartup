using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkFoodToHall : InteractWorkBase
{
    private MainLoopWorkContext context;
    
    public WorkFoodToHall(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }

    public void SetContext(MainLoopWorkContext context)
    {
        this.context = context;
    }
    
    public override void OnAssignWorker(WorkerBase worker)
    {
        base.OnAssignWorker(worker);
        // ITransformable porter = worker as ITransformable;
        // //TODO:Worker손에 음식 들려주기
    }
    protected override void HandlePostInteraction()
    {
        Debug.Log("Food to hall");
        // ITransformable porter = worker as ITransformable;
        // porter.DropPackage();
        //
    }
}