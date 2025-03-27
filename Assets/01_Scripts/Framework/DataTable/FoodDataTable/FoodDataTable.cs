using System.Collections.Generic;

public class FoodDataTable : DataTable
{
    public Dictionary<int, FoodData> Data = new();

    public FoodData GetFoodData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }

    public override void Load()
    {
        var result = LoadCsv<FoodData>("foodtable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.FoodID)) continue;
            Data.Add(row.FoodID, row);
        }
    }
}