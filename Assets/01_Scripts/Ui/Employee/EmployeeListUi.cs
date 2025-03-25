using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeeListUi : MonoBehaviour
{
    private TextMeshProUGUI text;
    public EmployeeManager employeeManager;
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void OnUpgradeButtonClick()
    {
        Debug.Log("OnUpgradeButtonClick »£√‚");
        employeeManager.UpgradeEmployee(text.text);
    }
}
