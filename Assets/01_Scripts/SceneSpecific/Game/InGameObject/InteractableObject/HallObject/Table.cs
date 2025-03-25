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
        Debug.Log("Job's Done");
    }
    

    public Table()
    {
    }
}