using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserDataManager : Singleton<UserDataManager>
{
    private UserData currentUserData;
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
        throw new NotImplementedException();
    }

    public bool LoadDB(string uid = "12345678")
    {
        throw new NotImplementedException();
    }

    public void InitCurrentUserData()
    {
        if(!LoadDB())
        {
            return;
        }
    }
}

