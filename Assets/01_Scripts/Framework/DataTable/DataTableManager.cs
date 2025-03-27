using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new();

    static DataTableManager()
    {
        var employeeDataTabletable = new EmployeeDataTable();
        employeeDataTabletable.Load();
        tables.Add(DataTableIds.Employee.ToString(), employeeDataTabletable);

        var foodDataTabletable = new FoodDataTable();
        foodDataTabletable.Load();
        tables.Add(DataTableIds.Food.ToString(), foodDataTabletable);
    }

    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
        {
            Debug.LogError($"Not found table with id: {id}");
            return null;
        }

        return tables[id] as T;
    }
}