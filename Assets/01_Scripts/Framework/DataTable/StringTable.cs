using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EmployeeDataTable : DataTable
{
    public Dictionary<int, EmployeeData> Data;
    public override void Load()
    {
        var result = LoadCsv<EmployeeData>("EmployeeData");
        foreach (var item in result)
        {
            if (Data.ContainsKey(item.StaffId))
            {
                continue;
            }
            Data.Add(item.StaffId, item);
        }
    }
}

public class EmployeeData
{
    public int StaffId;
    public int StaffNameKey;
    public int Description;
    
}


