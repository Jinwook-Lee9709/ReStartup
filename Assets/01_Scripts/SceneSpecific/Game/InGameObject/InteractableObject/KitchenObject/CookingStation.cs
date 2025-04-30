using UnityEngine;


public class CookingStation : InteractableObjectBase, IInterior
{
    [SerializeField] private SpriteRenderer objectRenderer;
    [SerializeField] private IconBubble iconBubble;
    public CookwareType cookwareType;
    
    
    
    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false)
    {
        iconBubble.ShowIcon(icon,iconBubble.transform.position, flipBackground);
        return true;
    }

    public override void HideIcon()
    {
        iconBubble.HideIcon();
        
    }

    public void ChangeSpirte(params Sprite[] sprite)
    {  
        objectRenderer.sprite = sprite[0];
    }
}