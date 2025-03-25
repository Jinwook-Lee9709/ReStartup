using System.Collections;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EmployeeDataTable : DataTable
{
    public Dictionary<int, EmployeeTableGetData> Data;
    public List<EmployeeTableGetData> DataList { get; private set; }

    public override void Load()
    {
        var result = LoadCsv<EmployeeTableGetData>(assetId: "stafftable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.StaffID))
            {
                continue;
            }
            Data.Add(row.StaffID, row);
            Debug.Log(row);
        }
    }
}