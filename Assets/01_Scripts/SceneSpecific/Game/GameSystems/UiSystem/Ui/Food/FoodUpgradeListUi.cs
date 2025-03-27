using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodUpgradeListUi : MonoBehaviour
{
    public GameObject upgradeItemObject;
    void Start()
    {
    }
    public void AddFoodUpgradeItem(FoodData data)
    {
        var ui = Instantiate(upgradeItemObject, transform).GetComponent<UiItem>();
        ui.Init(data);
    }
}
