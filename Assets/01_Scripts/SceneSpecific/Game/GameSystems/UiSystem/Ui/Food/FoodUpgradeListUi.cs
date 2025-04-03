using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodUpgradeListUi : MonoBehaviour
{
    public GameObject upgradeItemObject;
    public List<Button> allBuyButton;
    void Start()
    {
    }
    public void AddFoodUpgradeItem(FoodData data)
    {
        var ui = Instantiate(upgradeItemObject, transform).GetComponent<UiItem>();
        ui.Init(data);
    }
    public void AddButtonList(Button button)
    {
        allBuyButton.Add(button);
    }
    public void FoodAllBuy()
    {
        foreach (var item in allBuyButton)
        {
            item.onClick.Invoke();
        }
    }
}
