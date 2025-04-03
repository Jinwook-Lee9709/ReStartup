using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    
    public void FoodToTray()
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(Strings.Tray);
        handle.WaitForCompletion();
        var obj = foodPlacePivot.GetChild(0).GetComponent<FoodObject>();
        obj.SetSprite(handle.Result);
    }
    
}