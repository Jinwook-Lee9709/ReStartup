using System.Collections;
using System.Collections.Generic;

public class GuideCategoryDataTable : DataTable, IEnumerable<GuideCategoryData>
{
    public Dictionary<int, GuideCategoryData> Data = new();

    public GuideCategoryData GetData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }
    
    public override void Load()
    {
        var result = LoadCsv<GuideCategoryData>("guidecategorytable");
        foreach (var row in result)
        {
            Data.TryAdd(row.CategoryId, row);
        }
    }

    public IEnumerator<GuideCategoryData> GetEnumerator()
    {
        return Data.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
