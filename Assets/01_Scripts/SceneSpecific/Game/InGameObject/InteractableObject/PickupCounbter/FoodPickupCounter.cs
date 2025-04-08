using System;
using UnityEngine;

public class FoodPickupCounter : InteractableObjectBase
{
    [SerializeField] private Transform foodPlacePivot;

    public Transform FoodPlacePivot => foodPlacePivot;

    public GameObject LiftFood()
    {
        return foodPlacePivot.GetChild(0).gameObject;
    }

    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false )
    {
        return false;
    }

    public override void HideIcon()
    {
    }
}