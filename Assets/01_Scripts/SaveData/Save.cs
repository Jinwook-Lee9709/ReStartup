using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// DB�� ����� ������ ���� Ŭ����
/// </summary>
[Serializable]
public class UserData
{
    public string UID { get; set; }                                  //���� UID
    public string Name { get; set; }                                 //���� �̸�
    public int? Gold { get; set; }                                   //�ΰ��� ��ȭ
    public int? CurrentRankPoint { get; set; }                       //���� ��ŷ ����Ʈ
    public int? PositiveCnt { get; set; }                            //�ſ츸�� �մ� ī��Ʈ
    public int? NegativeCnt { get; set; }                            //�Ҹ��� �մ� ī��Ʈ
                                                                    
    public Dictionary<string, int> FoodSalesVolume { get; set; }     //Key : ����ID
                                                                     //Value : �ش� ���� �Ǹŷ�
    public Dictionary<string, int> EmployeeLevelValue { get; set; }  //Key : ����ID
                                                                     //Value : �ش� ������ ����

}

public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public float MasterVolume { get; set; } = 1f;
    public float BackGroundVolume { get; set; } = 1f;
    public float SFXVolume { get; set; } = 1f;
    public string UserToken { get; set; }
    public LanguageType LanguageType { get; set; } = LanguageType.Korean;
    public SaveDataV1()
    {
        Version = 1;
    }
    public override SaveData VersionUp()
    {
        throw new System.NotImplementedException();
    }
}