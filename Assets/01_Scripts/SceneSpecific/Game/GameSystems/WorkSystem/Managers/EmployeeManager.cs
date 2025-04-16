using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EmployeeManager
{
    private readonly string employeePrefab = "Agent.prefab";
    
    private Dictionary<int, EmployeeFSM> employees;
    private GameManager gameManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        var data = DataTableManager.Get<EmployeeDataTable>(DataTableIds.Employee.ToString()).Data;
        var saveData = UserDataManager.Instance.CurrentUserData.EmployeeSaveData;
        var query = saveData.Where(x => x.Value.level > 0);
        foreach(var pair in query)
        {
            InstantiateAndRegisterWorker(data[pair.Key]);
        }
    }

    public void AddEmployee(int id, EmployeeFSM employee)
    {
        gameManager.WorkerManager.RegisterWorker(employee, (WorkType)employee.EmployeeData.StaffType,
            employee.EmployeeData.StaffID);
    }
    
    public void InstantiateAndRegisterWorker(EmployeeTableGetData employeeData)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(employeePrefab);
        GameObject prefab = handle.WaitForCompletion();
        var newEmployee = Object.Instantiate(prefab).GetComponent<EmployeeFSM>();
        newEmployee.EmployeeData = employeeData;
        newEmployee.GetComponentInChildren<TextMeshPro>().text = $"{((WorkType)employeeData.StaffType).ToString()}직원"; 
        AddEmployee(employeeData.StaffID, newEmployee);

        LoadEmployeeSaveData(employeeData, newEmployee);
    }

    private static void LoadEmployeeSaveData(EmployeeTableGetData employeeData, EmployeeFSM newEmployee)
    {
        var save = UserDataManager.Instance.CurrentUserData.EmployeeSaveData[employeeData.StaffID];
        if (save.remainHp != employeeData.Health)
        {
            newEmployee.EmployeeData.currentHealth = save.remainHp;
        }
        else
        {
            newEmployee.EmployeeData.currentHealth = employeeData.Health;
        }
        newEmployee.AdjustTimer(save.remainHpDecreaseTime);
    }
}