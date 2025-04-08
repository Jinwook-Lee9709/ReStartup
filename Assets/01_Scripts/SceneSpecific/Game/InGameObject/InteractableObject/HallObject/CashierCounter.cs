using UnityEngine;
using UnityEngine.Serialization;

public class CashierCounter : InteractableObjectBase
{
    [SerializeField] private SpriteRenderer iconBackgroundRenderer;
    [SerializeField] private SpriteRenderer defaultIconRenderer;
    
    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false)
    {
        if (background != null)
        {
            iconBackgroundRenderer.gameObject.SetActive(true);
            iconBackgroundRenderer.sprite = background;
            iconBackgroundRenderer.flipX = flipBackground;
        }
        defaultIconRenderer.gameObject.SetActive(true);
        defaultIconRenderer.sprite = icon;

        return true;
    }

    public override void HideIcon()
    {
        iconBackgroundRenderer.gameObject.SetActive(false);
        defaultIconRenderer.gameObject.SetActive(false);
    }
}