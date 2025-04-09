using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
public enum TableType
{
    Main,
    Sub,
}

public class Table : InteractableObjectBase
{
    [SerializeField] private Transform foodPlacePivot;
    [SerializeField] private SpriteRenderer iconBackgroundRenderer;
    [SerializeField] private SpriteRenderer defaultIconRenderer;
    [SerializeField] private SpriteRenderer consumerRenderer;
    [SerializeField] private Table pairTable;
    
    [SerializeField] private TableType tableType;
    public Transform FoodPlacePivot => foodPlacePivot;
    public TableType TableType => tableType;
    public Table PairTable => pairTable;
    
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
        SpriteRenderer activeRenderer = null;
        SpriteRenderer inactiveRenderer = null;
        
        switch (pivot)
        {
            case IconPivots.Default:
                targetRendererTransform = defaultIconRenderer.transform;
                activeRenderer = defaultIconRenderer;
                inactiveRenderer = consumerRenderer;
                break;
            case IconPivots.Consumer:
                targetRendererTransform = consumerRenderer.transform;
                activeRenderer = consumerRenderer;
                inactiveRenderer = defaultIconRenderer;
                break;
            default:
                targetRendererTransform = defaultIconRenderer.transform;
                activeRenderer = defaultIconRenderer;
                inactiveRenderer = consumerRenderer;
                break;
        }

        if (background != null)
        {
            iconBackgroundRenderer.gameObject.SetActive(true);
            iconBackgroundRenderer.sprite = background;
            iconBackgroundRenderer.flipX = flipBackground;
            iconBackgroundRenderer.transform.position = targetRendererTransform.position;
        }
        if (activeRenderer != null)
        {
            activeRenderer.sprite = icon;
            activeRenderer.gameObject.SetActive(true);
        }

        if (inactiveRenderer != null)
        {
            inactiveRenderer.gameObject.SetActive(false);
        }

        return true;
    }
    
    public override void HideIcon()
    {
        iconBackgroundRenderer.gameObject.SetActive(false);
        defaultIconRenderer.gameObject.SetActive(false);
        consumerRenderer.gameObject.SetActive(false);
    }
}