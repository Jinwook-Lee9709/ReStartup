using UnityEngine;
using UnityEngine.AddressableAssets;

public class IconBubble : MonoBehaviour
{
    private static readonly string backgroundSpriteId = "Bubble";
    [SerializeField] private Transform baseTransform;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer fillSatisfaction;
    [SerializeField] private SpriteRenderer icon;

    private void Start()
    {
        background.sprite = Addressables.LoadAssetAsync<Sprite>(backgroundSpriteId).WaitForCompletion();
    }


    public void ShowIcon(Sprite sprite, Vector3 position, bool flip = false)
    {
        SetIcon(sprite);
        //FillingSatisfation(1f);
        SetColorSatisfaction(fillSatisfaction.material.color, Color.white, 1f);
        background.flipX = flip;
        fillSatisfaction.flipX = flip;
        baseTransform.position = position;
        baseTransform.PopupAnimation();

    }

    public void HideIcon()
    {
        baseTransform.PopdownAnimation();
    }

    private void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void FillingSatisfation(float fillAmount)
    {
        fillSatisfaction.material.SetFloat("_FillAmount", fillAmount);
    }

    public void SetColorSatisfaction(Color prevColor,Color targetColor, float normarizedAmount)
    {
        fillSatisfaction.material.color = Utills.LerpTowardWhiter(prevColor, targetColor, normarizedAmount);
    }
}