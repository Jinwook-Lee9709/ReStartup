using System;
using UnityEngine;

public class FoodPickupCounter : InteractableObjectBase
{
    [SerializeField] private Transform foodPlacePivot;
    [SerializeField] private IconBubble iconBubble;

    public Transform FoodPlacePivot => foodPlacePivot;

    public GameObject LiftFood()
    {
        return foodPlacePivot.GetChild(0).gameObject;
    }

    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false )
    {
        iconBubble.ShowIcon(icon, iconBubble.transform.position, flipBackground);
        return true;
    }

    public override void HideIcon()
    {
        iconBubble.HideIcon();
    }
}