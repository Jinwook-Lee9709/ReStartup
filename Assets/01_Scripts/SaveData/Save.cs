using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// DB에 저장될 유저의 정보 클래스
/// </summary>
[Serializable]
public class UserData
{
    public string UID { get; set; }
    public string Name { get; set; }
}

/// <summary>
/// 로컬에 저장할 유저의 게임 설정 클래스
/// </summary>
[Serializable]
public class SettingData
{
    public float MasterVolume { get; set; } = 1f;
    public float BackGroundVolume { get; set; } = 1f;
    public float SFXVolume { get; set; } = 1f;
    /// <summary>
    /// Define 머지 후 언어 타입 enum으로 변경
    /// </summary>
    public int LanguageType { get; set; } = 0;
}


public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public SettingData settingData;
    public SaveDataV1()
    {
        Version = 1;
    }
    public override SaveData VersionUp()
    {
        throw new System.NotImplementedException();
    }
}