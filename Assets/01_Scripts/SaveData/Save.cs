using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// DB�� ����� ������ ���� Ŭ����
/// </summary>
[Serializable]
public class UserData
{
    public string id { get; set; } = null;
    public string email { get; set; }
    public string login_source { get; set; }
}


/// <summary>
/// ���ÿ� ������ ������ ���� ���� Ŭ����
/// </summary>
[Serializable]
public class SettingData
{
    public float MasterVolume { get; set; } = 1f;
    public float BackGroundVolume { get; set; } = 1f;
    public float SFXVolume { get; set; } = 1f;
    /// <summary>
    /// Define ���� �� ��� Ÿ�� enum���� ����
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