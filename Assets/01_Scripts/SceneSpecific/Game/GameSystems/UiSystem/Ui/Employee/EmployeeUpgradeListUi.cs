using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeUpgradeListUi : MonoBehaviour
{
    private readonly string hallStaff = "HallStaff";
    private readonly string kitchenStaff = "KitchenStaff";
    private readonly string cashier = "CashierStaff";
    public GameObject upgradeItemObject;
    private EmployeeTableGetData employeeData;
    public Transform contents;
    private WorkType workType;
    
    public void AddEmployeeUpgradeItem(EmployeeTableGetData data, EmployeeUpgradePopup employeeUpgradePopup)
    {
        employeeData = data;
        var ui = Instantiate(upgradeItemObject, contents).GetComponent<EmployeeUIItem>();
        ui.Init(data, employeeUpgradePopup);

        if(employeeData.StaffID == 101201)
        {
            ui.gameObject.AddComponent<TutorialSelectionTarget>().Key = "EmployeeTutorial";
        }
    }
    public void SetWorkType(WorkType worktype)
    {
        switch (worktype)
        {
            case WorkType.All:
                break;
            case WorkType.Payment:
                GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(cashier);
                break;
            case WorkType.Hall:
                GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(hallStaff);
                break;
            case WorkType.Kitchen:
                GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(kitchenStaff);
                break;
        }
    }
}