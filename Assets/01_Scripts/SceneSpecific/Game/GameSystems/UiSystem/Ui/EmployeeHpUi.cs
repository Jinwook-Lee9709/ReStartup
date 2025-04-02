using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeHpUi : MonoBehaviour
{
    public GameObject employeeHpItem;
    public Transform employeeHpItemParent;
    private List<EmployeeHpUIItem> items = new();
    public void SetEmployeeUIItem(EmployeeTableGetData data)
    {
        var item = Instantiate(employeeHpItem, employeeHpItemParent).GetComponent<EmployeeHpUIItem>();
        items.Add(item);
        item.SetEmployeeHpUiItem(data);
    }
    public void EmployeeHpSet(EmployeeFSM employee)
    {
        foreach (var item in items)
        {
            if(item.employeeData == employee.EmployeeData)
            {
                item.EmployeeHpSet();
            }
        }
    }
}
