using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new();

    static DataTableManager()
    {
        var employeeDataTabletable = new EmployeeDataTable();
        employeeDataTabletable.Load();
        tables.Add(DataTableIds.Employee.ToString(), employeeDataTabletable);

        var foodDataTabletable = new FoodDataTable();
        foodDataTabletable.Load();
        tables.Add(DataTableIds.Food.ToString(), foodDataTabletable);

        var consumerDataTable = new ConsumerDataTable();
        consumerDataTable.Load();
        tables.Add(DataTableIds.Consumer.ToString(), consumerDataTable);

        var rankingDataTable = new RankingDataTable();
        rankingDataTable.Load();
        tables.Add(DataTableIds.Ranking.ToString(), rankingDataTable);

        var interiorDataTable = new InteriorDataTable();
        interiorDataTable.Load();
        tables.Add(DataTableIds.Interior.ToString(), interiorDataTable);
        
        var cookwareDataTable = new CookwareDataTable();
        cookwareDataTable.Load();
        tables.Add(DataTableIds.Cookware.ToString(), cookwareDataTable);

        var buffDataTable = new BuffDataTable();
        buffDataTable.Load();
        tables.Add(DataTableIds.Buff.ToString(), buffDataTable);

        var promotionDataTable = new PromotionDataTable();
        promotionDataTable.Load();
        tables.Add(DataTableIds.Promoiton.ToString(), promotionDataTable);

        var rankConditionDataTable = new RankConditionDataTable();
        rankConditionDataTable.Load();
        tables.Add(DataTableIds.rankCondition.ToString(), rankConditionDataTable);
    }

    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
        {
            Debug.LogError($"Not found table with id: {id}");
            return null;
        }

        return tables[id] as T;
    }
}