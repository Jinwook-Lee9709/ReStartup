using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeListUi : MonoBehaviour
{
    public EmployeeManager employeeManager;
    public GameObject upgradeObject;
    void Start()
    {
    }
    public void AddUpgrade(EmployeeTableGetData data)
    {
        var ui = Instantiate(upgradeObject, transform).GetComponent<UiItem>();
        ui.Init(data);
    }
}
