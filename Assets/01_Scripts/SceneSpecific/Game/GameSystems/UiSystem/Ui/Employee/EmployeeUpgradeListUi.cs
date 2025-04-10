using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeUpgradeListUi : MonoBehaviour
{
    public GameObject upgradeItemObject;
    private EmployeeTableGetData employeeData;
    public Transform contents;
    private WorkType workType;

    void Start()
    {

    }
    public void AddEmployeeUpgradeItem(EmployeeTableGetData data, EmployeeUpgradePopup employeeUpgradePopup)
    {
       
        employeeData = data;
        var ui = Instantiate(upgradeItemObject, contents).GetComponent<EmployeeUIItem>();
        ui.Init(data, employeeUpgradePopup);
    }
    public void SetWorkType(WorkType worktype)
    {
        switch (worktype)
        {
            case WorkType.All:
                break;
            case WorkType.Payment:
                GetComponentInChildren<TextMeshProUGUI>().text = "계산원";
                break;
            case WorkType.Hall:
                GetComponentInChildren<TextMeshProUGUI>().text = "홀직원";
                break;
            case WorkType.Kitchen:
                GetComponentInChildren<TextMeshProUGUI>().text = "주방직원";
                break;
        }
    }
}