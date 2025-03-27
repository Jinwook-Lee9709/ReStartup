using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class WorkCleanTable : InteractWorkBase
{
    private WorkFlowController controller;
    private FoodObject foodObject; 
    
    public WorkCleanTable(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }

    public void SetContext(WorkFlowController controller)
    {
        this.controller = controller;
    }

    public void SetFood(FoodObject foodObject)
    {
        this.foodObject = foodObject;
    }
    
    protected override void HandlePostInteraction()
    {
        Table table = target as Table;
        table.OnCleaned();
        controller.ReturnTable(table);
    }

}
