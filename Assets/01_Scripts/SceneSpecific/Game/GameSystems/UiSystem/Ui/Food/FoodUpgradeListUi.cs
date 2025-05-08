using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FoodUpgradeListUI : MonoBehaviour
{
    public GameObject upgradeItemObject;
    private Dictionary<int, FoodUpgradeUIItem> foodUpgradeUIItems = new();

    
    
    void Start()
    {
        float newSize = Screen.width * 0.15f;
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(newSize, newSize);

        UserDataManager.Instance.ChangeMoneyAction += UpdatePayable;
    }

    void Update()
    {
        float newSize = Screen.width * 0.15f;
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(newSize, newSize);
    }

    private void OnDestroy()
    {
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.ChangeMoneyAction -= UpdatePayable;
        }
    }

    public void AddFoodUpgradeItem(FoodData data, FoodUpgradePopup popup)
    {
        var ui = Instantiate(upgradeItemObject, transform).GetComponent<FoodUpgradeUIItem>();
        ui.Init(data, popup);

        foodUpgradeUIItems.Add(data.FoodID, ui);
    }

    public void UnlockFood(FoodData data)
    {
        foreach (var pair in foodUpgradeUIItems)
        {
            if (pair.Key == data.FoodID)
            {
                pair.Value.UnlockFoodUpgrade();
            }
        }
    }

    public void UpdatePayable(int? currentMoney)
    {
        foreach (var pair in foodUpgradeUIItems)
        {
            pair.Value.UpdatePayableArrow((int)currentMoney);
        }
    }
}