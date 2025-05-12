using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideElementDataTable : DataTable, IEnumerable<GuideElementData>
{
    public Dictionary<int, GuideElementData> Data = new();

    public GuideElementData GetData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }
    
    public override void Load()
    {
        var result = LoadCsv<GuideElementData>("guideCategory");
        foreach (var row in result)
        {
            Data.TryAdd(row.EntryID, row);
        }
    }

    public IEnumerator<GuideElementData> GetEnumerator()
    {
        return Data.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
