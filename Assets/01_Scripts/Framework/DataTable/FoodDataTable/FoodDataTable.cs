using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FoodDataTable : DataTable, IEnumerable<FoodData>
{
    public Dictionary<int, FoodData> Data = new();

    public FoodData GetFoodData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }

    public List<KeyValuePair<int,FoodData>> GetSceneFoodDataList(ThemeIds themeId)
    {
        return Data.Where(x => x.Value.Type == (int)themeId).ToList();
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

    public IEnumerator<FoodData> GetEnumerator()
    {
        return Data.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}