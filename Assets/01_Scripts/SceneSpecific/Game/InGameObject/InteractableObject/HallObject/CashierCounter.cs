using UnityEngine;
using UnityEngine.Serialization;

public class CashierCounter : InteractableObjectBase, IInterior
{
    [SerializeField] private SpriteRenderer objectRenderer;
    [SerializeField] private IconBubble iconBubble;
    
    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false)
    {
        iconBubble.ShowIcon(icon,iconBubble.transform.position, flipBackground);
        return true;
    }

    public override void HideIcon()
    {
        iconBubble.HideIcon();
        
    }

    public void ChangeSpirte(Sprite sprite)
    {
        objectRenderer.sprite = sprite;
    }
}