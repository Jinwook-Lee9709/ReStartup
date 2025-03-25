using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    private Dictionary<string, EmployeeFSM> employeeDictionary = new Dictionary<string, EmployeeFSM>();
    private Dictionary<string, EmployeeFSM> EmployeeDictionary { get { return employeeDictionary; } }
    public void AddEmployee(string name, EmployeeFSM employee)
    {
        EmployeeDictionary.Add(name, employee);
        Debug.Log("EmployeeManager 호출");
        Debug.Log(name);
    }
    public void UpgradeEmployee(string name)
    {
        if (EmployeeDictionary.ContainsKey(name))
        {
            //EmployeeDictionary[name].OnUpgrade();
        }
        else
        {
            Debug.LogError("업그레이드 이름 없음");
        }

    }
}
