using UnityEngine;

public class FoodPickupCounter : InteractableObjectBase
{
    [SerializeField] private Transform foodPlacePivot;

    public Transform FoodPlacePivot => foodPlacePivot;

    public GameObject LiftFood()
    {
        return foodPlacePivot.GetChild(0).gameObject;
    }
}