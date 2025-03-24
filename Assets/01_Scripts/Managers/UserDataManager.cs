using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
        throw new System.NotImplementedException();
    }

    public async Task<bool> LoadDB(string uid = "1234")
    {
        var result = await UserDataDAC.GetUserData(uid);
        CurrentUserData = result;
        return true;
    }

    public void InitCurrentUserData()
    {
        StartCoroutine(LoadRoutine());
    }

    IEnumerator LoadRoutine()
    {
        Task<bool> task = LoadDB();
        while (!task.IsCompleted)
        {
            yield return new WaitForEndOfFrame();

        }
        if(task.Result)
        {
            Debug.Log("Load DB Success");
        }
        else
        {
            Debug.Log("Load DB Fail");
        }
    }
}

