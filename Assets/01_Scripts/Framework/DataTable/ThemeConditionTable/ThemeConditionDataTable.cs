using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeConditionDataTable : DataTable
{
    public Dictionary<int, ThemeConditionData> Data = new();

    public ThemeConditionData GetConditionData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }

    public override void Load()
    {
        var result = LoadCsv<ThemeConditionData>("themeconditiontable");
        foreach (var row in result)
        {
            Data.TryAdd(row.ThemeID, row);
        }
    }
}
