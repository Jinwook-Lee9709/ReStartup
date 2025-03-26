using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class WorkCleanTable : InteractWorkBase
{
    private WorkFlowController controller;
    public WorkCleanTable(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }

    public void SetContext(WorkFlowController controller)
    {
        this.controller = controller;
    }
    
    protected override void HandlePostInteraction()
    {
        Table table = target as Table;
        table.OnCleaned();
        controller.ReturnTable(table);
    }
}
