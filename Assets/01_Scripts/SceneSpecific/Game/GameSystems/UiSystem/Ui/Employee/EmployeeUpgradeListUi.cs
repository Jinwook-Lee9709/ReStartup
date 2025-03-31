using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeUpgradeListUi : MonoBehaviour
{
    public GameObject upgradeItemObject;
    public EmployeeTableGetData employeeData;
    public List<Button> allBuyButton;


    void Start()
    {
    }
    public void AddEmployeeUpgradeItem(EmployeeTableGetData data)
    {
        var ui = Instantiate(upgradeItemObject, transform).GetComponent<UiItem>();
        ui.Init(data);
    }
    public void AddButtonList(Button button)
    {
        allBuyButton.Add(button);
    }
    public void EmployeeAllBuy()
    {
        foreach (var item in allBuyButton)
        {
            item.onClick.Invoke();
        }
    }
}