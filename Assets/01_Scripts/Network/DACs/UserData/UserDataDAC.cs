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

    public static async Task<Dictionary<int, FoodSaveData>> GetFoodData(string uid)
    {
        var result = await RestApiService.GetAsync<Dictionary<int, FoodSaveData>>($"https://localhost:443/api/getFoodData?uuid={uid}");
        return result;
    }

    public static async Task<bool> PostFoodData(string uid, Dictionary<int, FoodSaveData> data)
    {
        String json =  Newtonsoft.Json.JsonConvert.SerializeObject(data);
        var result = await RestApiService.PostAsync<bool>($"https://localhost:443/api/postFoodData?uuid={uid}", json);
        return result;
    }
}
