using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorDataTable : DataTable, IEnumerable<InteriorData>
{
    public static readonly string FileName = "interiortable";
    
    public Dictionary<int, InteriorData> Data = new();

    public InteriorData GetData(int id)
    {
        Data.TryGetValue(id, out InteriorData data);
        return data;
    }
    
    public override void Load()
    {
        var result = LoadCsv<InteriorData>(FileName);
        foreach (var row in result)
        {
            Data.TryAdd(row.InteriorID, row);
        }
    }

    public IEnumerator<InteriorData> GetEnumerator()
    {
        return Data.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
