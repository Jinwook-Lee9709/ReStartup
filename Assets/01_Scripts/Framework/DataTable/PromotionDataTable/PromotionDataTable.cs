using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionDataTable : DataTable
{
    public Dictionary<int, PromotionBase> Data = new();

    public Dictionary<int,PromotionBase> GetData()
    {
        return Data;
    }

    public override void Load()
    {
        var result = LoadCsv<PromotionBase>("promotiontable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.PromotionID)) 
                continue;
            Data.Add(row.PromotionID, row);
        }
    }
}
