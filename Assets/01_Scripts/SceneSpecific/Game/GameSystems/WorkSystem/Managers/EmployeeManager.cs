using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public class EmployeeManager : IDisposable
{
    private readonly string employeePrefab = "Employee{0}";
    
    private Dictionary<int, EmployeeFSM> employees = new();
    private GameManager gameManager;
    private BuffManager buffManager;

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

    public void Start()
    {
        buffManager = gameManager.buffManager;
        buffManager.OnBuffUsed += OnBuffUsed;
        buffManager.OnBuffExpired += OnBuffExpired;
    }

    private void OnBuffUsed(Buff buff)
    {
        switch (buff.BuffType)
        {
            case BuffType.StaffMove:
            {
                foreach (var employee in employees.Values)
                {
                    employee.ApplyMoveSpeedBuff(buff.BuffEffect);
                }
                break;
            }
            case BuffType.StaffWork:
            {
                foreach (var employee in employees.Values)
                {
                    employee.ApplyWorkSpeedBuff(buff.BuffEffect);
                }
                break;
            }
            default:
                break;
        }
    }

    private void OnBuffExpired(Buff buff)
    {
        switch (buff.BuffType)
        {
            case BuffType.StaffMove:
            {
                foreach (var employee in employees.Values)
                {
                    employee.RemoveMoveSpeedBuff();
                }
                break;
            }
            case BuffType.StaffWork:
            {
                foreach (var employee in employees.Values)
                {
                    employee.RemoveWorkSpeedBuff();
                }
                break;
            }
        }
    }

    public void UpgradeEmployee(int id)
    {
        var employee = employees[id];
        employee.OnUpgrade();
    }

    public void AddEmployee(int id, EmployeeFSM employee)
    {
        gameManager.WorkerManager.RegisterWorker(employee, (WorkType)employee.EmployeeData.StaffType,
            employee.EmployeeData.StaffID);
    }
    
    public void InstantiateAndRegisterWorker(EmployeeTableGetData employeeData)
    {
        var assetId = String.Format(employeePrefab, employeeData.StaffID);
        var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
        GameObject prefab = handle.WaitForCompletion();
        var newEmployee = Object.Instantiate(prefab).GetComponent<EmployeeFSM>();
        newEmployee.EmployeeData = employeeData;
        AddEmployee(employeeData.StaffID, newEmployee);

        LoadEmployeeSaveData(employeeData, newEmployee);
        employees[employeeData.StaffID] = newEmployee;
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


    public void Dispose()
    {
        buffManager.OnBuffUsed -= OnBuffUsed;
        buffManager.OnBuffExpired -= OnBuffExpired;
    }
}