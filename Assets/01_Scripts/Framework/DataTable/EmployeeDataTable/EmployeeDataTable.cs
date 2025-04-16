using System.Collections;
using System.Collections.Generic;

public class EmployeeDataTable : DataTable, IEnumerable<EmployeeTableGetData>
{
    public Dictionary<int, EmployeeTableGetData> Data = new();
    public List<EmployeeTableGetData> DataList { get; private set; }

    public override void Load()
    {
        var result = LoadCsv<EmployeeTableGetData>("stafftable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.StaffID)) continue;
            Data.Add(row.StaffID, row);
        }
    }
    public IEnumerator<EmployeeTableGetData> GetEnumerator()
    {
        return Data.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}