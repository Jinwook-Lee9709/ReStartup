using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffDataTable : DataTable
{
    public Dictionary<int, Buff> Data = new();

    public Buff GetBuffForBuffID(int buffID)
    {
        Data.TryGetValue(buffID, out var buff);
        Buff result = new(buff);
        return result;
    }

    public override void Load()
    {
        var result = LoadCsv<Buff>("bufftable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.BuffID)) continue;
            Data.Add(row.BuffID, row);
        }
    }
}
