using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookwareDataTable : DataTable, IEnumerable<CookwareData>
{
    public static readonly string FileName = "CookingwareTable";
    
    public Dictionary<int, CookwareData> Data = new();

    public CookwareData GetData(int id)
    {
        Data.TryGetValue(id, out CookwareData data);
        return data;
    }
    
    public override void Load()
    {
        var result = LoadCsv<CookwareData>(FileName);
        foreach (var row in result)
        {
            Data.TryAdd(row.InteriorID, row);
        }
    }
    
    public List<KeyValuePair<int,CookwareData>> GetSceneFoodDataList(ThemeIds themeId)
    {
        return Data.Where(x => x.Value.RestaurantType == (int)themeId).ToList();
    }


    public IEnumerator<CookwareData> GetEnumerator()
    {
        return Data.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
