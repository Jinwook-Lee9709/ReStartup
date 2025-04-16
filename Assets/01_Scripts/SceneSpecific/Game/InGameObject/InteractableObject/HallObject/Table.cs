using UnityEngine;
using UnityEngine.AddressableAssets;

public enum TableType
{
    Main,
    Sub
}

public class Table : InteractableObjectBase, IInterior
{
    [SerializeField] private Transform foodPlacePivot;
    [SerializeField] private SpriteRenderer objectRenderer;
    [SerializeField] private Transform defaultIconTransform;
    [SerializeField] private Transform consumerIconTransform;
    [SerializeField] private IconBubble iconBubble;
    [SerializeField] private Table pairTable;

    [SerializeField] private TableType tableType;
    public Transform FoodPlacePivot => foodPlacePivot;
    public TableType TableType => tableType;
    public Table PairTable => pairTable;
    public IconBubble IconBubble => iconBubble;
    public void ChangeSpirte(Sprite sprite)
    {
        objectRenderer.sprite = sprite;
    }

    public GameObject GetFood()
    {
        var food = foodPlacePivot.GetChild(0).gameObject;
        return food;
    }

    public void FoodToTray()
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(Strings.Tray);
        handle.WaitForCompletion();
        var obj = foodPlacePivot.GetChild(0).GetComponent<FoodObject>();
        obj.SetSprite(handle.Result);
    }

    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false)
    {
        Transform targetRendererTransform = null;

        switch (pivot)
        {
            case IconPivots.Default:
                targetRendererTransform = defaultIconTransform.transform;
                break;
            case IconPivots.Consumer:
                targetRendererTransform = consumerIconTransform.transform;
                break;
            default:
                targetRendererTransform = defaultIconTransform.transform;
                break;
        }

        iconBubble.ShowIcon(icon, targetRendererTransform.position, flipBackground);
        return true;
    }

    public override void HideIcon()
    {
        iconBubble.HideIcon();
    }
}