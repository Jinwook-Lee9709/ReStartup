using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Table : InteractableObjectBase
{
    [SerializeField] private Transform foodPlacePivot;
    public Transform FoodPlacePivot => foodPlacePivot;
    
    
    public override void OnInteractCompleted()
    {
        base.OnInteractCompleted();
    }

    public GameObject GetFood()
    {
        var food = foodPlacePivot.GetChild(0).gameObject;
        return food;
    }

    public void OnCleaned()
    {
        Destroy(foodPlacePivot.GetChild(0).gameObject);
    }
}