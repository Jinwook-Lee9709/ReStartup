using System;
using System.Collections.Generic;

/// <summary>
///     DB�� ����� ������ ���� Ŭ����
/// </summary>
[Serializable]
public class UserData
{
    public string UID { get; set; } //���� UID
    public string Name { get; set; } //���� �̸�
    public int? Gold { get; set; } = new(); //�ΰ��� ��ȭ
    public int? CurrentRankPoint { get; set; } //���� ��ŷ ����Ʈ
    public int? PositiveCnt { get; set; } //�ſ츸�� �մ� ī��Ʈ
    public int? NegativeCnt { get; set; } //�Ҹ��� �մ� ī��Ʈ

    public Dictionary<int, FoodSaveData> FoodSaveData { get; set; } = new()
    {
        { 301001, new FoodSaveData() },
        { 301002, new FoodSaveData() },
        { 301003, new FoodSaveData() }
    };

    public Dictionary<string, int> EmployeeLevelValue { get; set; }
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