using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodQuestDataTable : DataTable
{
    public Dictionary<int, PeriodQuestData> Data = new();

public PeriodQuestData GetFoodData(int id)
{
    Data.TryGetValue(id, out var data);
    return data;
}

public override void Load()
{
    var result = LoadCsv<PeriodQuestData>("periodquesttable");
    foreach (var row in result)
    {
            if (Data.ContainsKey(row.PeriodQuestID)) continue;
        Data.Add(row.PeriodQuestID, row);
    }
}
}
