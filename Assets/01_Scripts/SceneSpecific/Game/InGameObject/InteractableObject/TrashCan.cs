using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : InteractableObjectBase, IInterior
{
    
    [SerializeField] private SpriteRenderer objectRenderer;
    [SerializeField] private IconBubble iconBubble;
    
    public InteractStatus InteractStatus { get; }
    public float InteractProgress { get; }
    
    public void ChangeSpirte(params Sprite[] sprite)
    {
        objectRenderer.sprite = sprite[0];
    }
    
    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false)
    {
        iconBubble.ShowIcon(icon, iconBubble.transform.position, flipBackground);
        return true;
    }

    public override void HideIcon()
    {
        iconBubble.HideIcon();
    }

}
