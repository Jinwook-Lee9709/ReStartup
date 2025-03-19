using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new();

    static DataTableManager()
    {

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