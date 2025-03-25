using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    [SerializeField] private List<EmployeeFSM> employeeList = new List<EmployeeFSM>();

    public EmployeeListUi employeeListUi;
    public void AddEmployee(EmployeeFSM employee)
    {
        employeeList.Add(employee);
        employeeListUi.AddUpgradeList(employee);
        Debug.Log("EmployeeManager »£√‚");
        Debug.Log(name);
    }
}
