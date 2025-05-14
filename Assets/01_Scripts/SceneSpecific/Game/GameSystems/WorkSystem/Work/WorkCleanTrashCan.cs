using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkCleanTrashCan : InteractWorkBase
{
    private WorkFlowController controller;
    private ObjectArea area;
    
    public WorkCleanTrashCan(WorkManager workManager, WorkType workType, float interactTime = 1, bool isInteruptible = true, bool isStoppable = true) : base(workManager, workType, interactTime, isInteruptible, isStoppable)
    {
    }

    public void SetContext(WorkFlowController controller, ObjectArea area)
    {
        this.controller = controller;
        this.area = area;
    }

    public override void OnWorkRegistered()
    {
        var iconHandle = Addressables.LoadAssetAsync<Sprite>(Strings.TrashIcon);
        var backgroundHandle = Addressables.LoadAssetAsync<Sprite>(Strings.Bubble);

        iconHandle.WaitForCompletion();
        backgroundHandle.WaitForCompletion();
        
        Sprite iconSprite = iconHandle.Result;
        Sprite backgroundSprite = backgroundHandle.Result;
        
        var trashCan = target as TrashCan;
        trashCan.ShowIcon(IconPivots.Default, iconSprite, backgroundSprite, false);
    }
    
    protected override void HandlePostInteraction()
    {
        var trashCan = target as TrashCan;
        AudioManager.Instance.PlaySFX("CleanRubbish");
        trashCan?.HideIcon();
        controller.OnCleanTrash(area);
        worker.ClearWork();
    }
}
