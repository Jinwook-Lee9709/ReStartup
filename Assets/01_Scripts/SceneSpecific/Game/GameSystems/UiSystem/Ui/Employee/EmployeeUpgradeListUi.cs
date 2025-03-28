using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeUpgradeListUi : MonoBehaviour
{
    public GameObject upgradeItemObject;
    void Start()
    {
    }
    public void AddEmployeeUpgradeItem(EmployeeTableGetData data)
    {
        var ui = Instantiate(upgradeItemObject, transform).GetComponent<UiItem>();
        ui.Init(data);
    }
}