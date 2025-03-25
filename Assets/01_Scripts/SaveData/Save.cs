using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// DB에 저장될 유저의 정보 클래스
/// </summary>
[Serializable]
public class UserData
{
    public string UID { get; set; }                                  //유저 UID
    public string Name { get; set; }                                 //유저 이름
    public int? Gold { get; set; }                                   //인게임 재화
    public int? CurrentRankPoint { get; set; }                       //현재 랭킹 포인트
    public int? PositiveCnt { get; set; }                            //매우만족 손님 카운트
    public int? NegativeCnt { get; set; }                            //불만족 손님 카운트
                                                                    
    public Dictionary<string, int> FoodSalesVolume { get; set; }     //Key : 음식ID
                                                                     //Value : 해당 음식 판매량
    public Dictionary<string, int> EmployeeLevelValue { get; set; }  //Key : 직원ID
                                                                     //Value : 해당 직원의 레벨

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