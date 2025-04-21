using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class FoodSaveDataDAC : MonoBehaviour
{
    public static async UniTask<ApiResponse<FoodSaveData[]>> GetFoodData(int themeId)
    {
        var payload = new Dictionary<string, string>();
        payload.Add("theme", themeId.ToString());
        var data = await RestApiService.GetAsyncWithToken<FoodSaveData[]>(Endpoints.GetFoodByTheme, payload);
        return data;
    }
    
    public static async UniTask<bool> UpdateFoodData(FoodSaveData saveData)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["info"] = JsonConvert.SerializeObject(saveData)
        };
        ApiResponse<FoodSaveData[]> response = await RestApiService.PostAsyncWithToken<FoodSaveData[]>(Endpoints.SaveFoodUrl, data);
        return response != null && response.ResponseCode == ResponseType.Success;
    }

    public static async UniTask<bool> UpdateFoodData(List<FoodSaveData> saveData)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["info"] = JsonConvert.SerializeObject(saveData)
        };
        ApiResponse<FoodSaveData[]> response = await RestApiService.PostAsyncWithToken<FoodSaveData[]>(Endpoints.SaveFoodsUrl, data);
        return response != null && response.ResponseCode == ResponseType.Success;
    }
}
