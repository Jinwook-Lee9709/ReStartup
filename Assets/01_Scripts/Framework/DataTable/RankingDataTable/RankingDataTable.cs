using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingDataTable : DataTable
{
    public Dictionary<int, RankingData> Data = new();

    public RankingData GetFoodData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }

    public override void Load()
    {
        var result = LoadCsv<RankingData>("rankingsystemtable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.Ranking)) continue;
            Data.Add(row.Ranking, row);
        }
    }
}
