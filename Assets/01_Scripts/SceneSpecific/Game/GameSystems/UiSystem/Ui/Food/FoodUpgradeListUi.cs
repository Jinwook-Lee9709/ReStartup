using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FoodUpgradeListUI : MonoBehaviour
{
    public GameObject upgradeItemObject;
    public List<Button> allBuyButton;
    private Dictionary<int , FoodUpgradeUIItem> foodUpgradeUIItems = new();
    void Start()
    {
        float newSize = Screen.width * 0.15f;
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(newSize,newSize);
    }    void Update()
    {
        float newSize = Screen.width * 0.15f;
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(newSize,newSize);
    }
    public void AddFoodUpgradeItem(FoodData data, FoodUpgradePopup popup)
    {
        var ui = Instantiate(upgradeItemObject, transform).GetComponent<FoodUpgradeUIItem>();
        ui.Init(data, popup);
        
        foodUpgradeUIItems.Add(data.FoodID, ui);
    }
    public void AddButtonList(Button button)
    {
        allBuyButton.Add(button);
    }
    public void UnlockFood(FoodData data)
    {
        foreach (var pair in foodUpgradeUIItems)
        {
            if(pair.Key == data.FoodID)
            {
                pair.Value.UnlockFoodUpgrade();
            }
        }
    }
    public void FoodAllBuy()
    {
        foreach (var item in allBuyButton)
        {
            item.onClick.Invoke();
        }
    }
}
