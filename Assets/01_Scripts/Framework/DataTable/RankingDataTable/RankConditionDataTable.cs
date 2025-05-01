using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankConditionDataTable : DataTable, IEnumerable<RankConditionData>
{
    public Dictionary<int, RankConditionData> Data = new();

    public RankConditionData GetRankConditionData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }

    public override void Load()
    {
        var result = LoadCsv<RankConditionData>("rankconditiontable");
        foreach (var row in result)
        {   
            if (Data.ContainsKey(row.RangkingID)) continue;
            Data.Add(row.RangkingID, row);
        }
    }

    public IEnumerator<RankConditionData> GetEnumerator()
    {
        return Data.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
