using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDataTable : DataTable
{
    public Dictionary<int, MissionData> Data = new();

public MissionData GetFoodData(int id)
{
    Data.TryGetValue(id, out var data);
    return data;
}

public override void Load()
{
    var result = LoadCsv<MissionData>("missiontable");
    foreach (var row in result)
    {
            if (Data.ContainsKey(row.MissionId)) continue;
        Data.Add(row.MissionId, row);
    }
}
}
