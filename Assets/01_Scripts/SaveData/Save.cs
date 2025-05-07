using System;
using System.Collections.Generic;

/// <summary>
///     DB�� ����� ������ ���� Ŭ����
/// </summary>
[Serializable]
public class UserData
{
    public string UID { get; set; } 
    public string Name { get; set; } 
    public int? Money { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int AdTicket { get; set; } = 1;
    public int? CurrentRankPoint { get; set; }
    public int CurrentRank { get; set; }
    public int? PositiveCnt { get; set; } = new(); 
    public int? NegativeCnt { get; set; } = new();
    public int InflowRate { get; set; } = 0;
    public bool IsRankCompensationClaimed = false;
    public Dictionary<ThemeIds, (int positive, int negative)> reviewCountForTheme = new()
    {
        {ThemeIds.Theme1, (0,0) },
        {ThemeIds.Theme2, (0,0) },
        {ThemeIds.Theme3, (0,0) },
    };
    public long Cumulative { get; set; } = 0;
    public Dictionary<ThemeIds, StageStatusData> ThemeStatus { get; set; } = new();
    public Dictionary<int, FoodSaveData> FoodSaveData { get; set; } = new();

    public Dictionary<int, EmployeeSaveData> EmployeeSaveData { get; set; } = new();

    public Dictionary<ThemeIds, Dictionary<CookwareType, int>> CookWareUnlock { get; set; } = new()
    {
        {
            ThemeIds.Theme1, new Dictionary<CookwareType, int>
            {
                { CookwareType.CoffeeMachine, 0 },
                { CookwareType.DrinkingFountain, 0 },
                { CookwareType.SparklingWaterMaker, 0 },
                { CookwareType.Blender, 0 },
                { CookwareType.Oven, 0 },
                { CookwareType.SushiCountertop, 0 },
                { CookwareType.Fryer, 0 },
                { CookwareType.CharcoalGrill, 0 },
                { CookwareType.GriddleGrill, 0 },
                { CookwareType.Pot, 0 },
                { CookwareType.KitchenTable, 0 }
            }
        },
        { ThemeIds.Theme2, new Dictionary<CookwareType, int>() },
        { ThemeIds.Theme3, new Dictionary<CookwareType, int>() }
    };

    public Dictionary<int, int> InteriorSaveData { get; set; } = new();
    
    public Dictionary<int, PromotionData> PromotionSaveData { get; set; } = new();
    
    public Dictionary<int, BuffSaveData> BuffSaveData { get; set; } = new();
    
    public Dictionary<int,MissionSaveData> MissionSaveData { get; set; } = new();
    
    //Don't Use this
    public List<ReviewSaveData> ReviewSaveData { get; set; } = new();
}

public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public SaveDataV1()
    {
        Version = 1;
    }
    public float BackGroundVolume { get; set; } = 1f;
    public float SFXVolume { get; set; } = 1f;
    public string UserToken { get; set; }
    public LanguageType LanguageType { get; set; } = LanguageType.Korean;

    public override SaveData VersionUp()
    {
        throw new NotImplementedException();
    }
}