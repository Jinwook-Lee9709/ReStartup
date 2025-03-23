using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class UserDataDAC
{
    public static async Task<UserData> GetUserData(string uid)
    {
        var result = await RestApiService.GetAsync<UserData>($"https://localhost:443/api/getInfo?uuid={uid}");
        return result;
    }
}
