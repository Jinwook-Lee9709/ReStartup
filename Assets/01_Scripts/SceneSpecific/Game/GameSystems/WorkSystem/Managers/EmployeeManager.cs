using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    public EmployeeListUi employeeListUi;
    public void Awake()
    {

    }
    public void Start()
    {
        var data = DataTableManager.Get<EmployeeDataTable>("Employee").Data;
        foreach (var item in data.Values)
        {
            employeeListUi.AddUpgrade(item);
        }
    }
}
