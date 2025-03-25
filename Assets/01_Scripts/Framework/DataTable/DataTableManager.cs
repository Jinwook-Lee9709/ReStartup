using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new();

    static DataTableManager()
    {
        var table1 = new EmployeeDataTable();
        table1.Load();
        tables.Add("Employee", table1);
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