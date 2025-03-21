using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : Singleton<UserDataManager>
{
    private UserData currentUserData;
    private SettingData currentSettingData;
    public UserData CurrentUserData
    {
        get => currentUserData;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("currentUserInfo Set Value");
            }
            currentUserData = value;
        }
    }

    public bool SaveDB()
    {
        throw new System.NotImplementedException();
    }

    public bool LoadDB(string uid = "12345678")
    {
        throw new System.NotImplementedException();
    }

    public void InitCurrentUserData()
    {
        if(!LoadDB())
        {
            return;
        }
    }
}

