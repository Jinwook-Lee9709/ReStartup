using UnityEngine;


public class CookingStation : InteractableObjectBase
{
    public CookwareType cookwareType;

    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer defaultIconRenderer;
    
    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false)
    {
        defaultIconRenderer.gameObject.SetActive(true);
        defaultIconRenderer.sprite = icon;
        if (background != null)
        {
            backgroundRenderer.gameObject.SetActive(true);
            backgroundRenderer.sprite = background;
            backgroundRenderer.flipX = flipBackground;
        }

        return true;
    }

    public override void HideIcon()
    {
        defaultIconRenderer.gameObject.SetActive(false);
        backgroundRenderer.gameObject.SetActive(false);
        
    }
}