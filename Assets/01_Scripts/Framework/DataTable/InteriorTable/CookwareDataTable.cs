using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookwareDataTable : DataTable
{
    public static readonly string FileName = "CookingwareTable";
    
    public Dictionary<int, CookwareData> Data = new();

    public CookwareData GetData(int id)
    {
        Data.TryGetValue(id, out CookwareData data);
        return data;
    }
    
    public override void Load()
    {
        var result = LoadCsv<CookwareData>(FileName);
        foreach (var row in result)
        {
            Data.TryAdd(row.InteriorID, row);
        }
    }
}
