using System;
using System.Collections.Generic;

/// <summary>
///     DB�� ����� ������ ���� Ŭ����
/// </summary>
[Serializable]
public class UserData
{
    public string UID { get; set; } //���� UID
    public string Name { get; set; } = "Player"; //���� �̸�
    public int? Gold { get; set; } = 0; //�ΰ��� ��ȭ
    public int? CurrentRankPoint { get; set; } = 2000;//���� ��ŷ ����Ʈ
    public int? CurrentRank { get; set; } = new();
    public int? PositiveCnt { get; set; } = new(); //�ſ츸�� �մ� ī��Ʈ
    public int? NegativeCnt { get; set; } = new(); //�Ҹ��� �մ� ī��Ʈ\
    
    public Dictionary<int, FoodSaveData> FoodSaveData { get; set; } = new()
    {
        { 301001, new FoodSaveData() },
        { 301002, new FoodSaveData() },
        { 301003, new FoodSaveData() }
    };

    public Dictionary<string, int> EmployeeLevelValue { get; set; }

    public Dictionary<ThemeIds, Dictionary<CookwareType, int>> CookWareUnlock { get; set; } = new()
    {
        {
            ThemeIds.Theme1, new Dictionary<CookwareType, int>()
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
                { CookwareType.KitchenTable, 0 },
            }
        },
        { ThemeIds.Theme2, new Dictionary<CookwareType, int>() },
        { ThemeIds.Theme3, new Dictionary<CookwareType, int>() }
    };
    
    public Dictionary<int, int> InteriorSaveData { get; set; } = new();

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

    public float MasterVolume { get; set; } = 1f;
    public float BackGroundVolume { get; set; } = 1f;
    public float SFXVolume { get; set; } = 1f;
    public string UserToken { get; set; }
    public LanguageType LanguageType { get; set; } = LanguageType.Korean;

    public override SaveData VersionUp()
    {
        throw new NotImplementedException();
    }
}