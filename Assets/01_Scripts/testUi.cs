using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class testUi : MonoBehaviour
{
    public EmployeeData employeeData;

    public void Init(EmployeeFSM data)
    {
        employeeData = data.EmployeeData;
        var text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"{employeeData.name} : {employeeData.upgradeCount}";
        var button = GetComponentInChildren<Button>();
        button.onClick.AddListener(() =>
        {
            employeeData.upgradeCount++;
            text.text = $"{employeeData.name} : {employeeData.upgradeCount}";
            employeeData.Speed = employeeData.defultSpeed * employeeData.upgradeCount;
            data.StatsUpdate();
        });
    }
}
