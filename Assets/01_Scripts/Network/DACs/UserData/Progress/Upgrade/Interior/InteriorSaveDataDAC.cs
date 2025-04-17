using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class InteriorSaveDataDAC
{
    public static async UniTask<ApiResponse<InteriorSaveData[]>> GetInteriorData(int themeId)
    {
        var payload = new Dictionary<string, string>();
        payload.Add("theme", themeId.ToString());
        var data = await RestApiService.GetAsyncWithToken<ApiResponse<InteriorSaveData[]>>(Endpoints.GetInteriorByTheme, payload);
        return data;
    }

    public static async UniTask<bool> UpdateInteriorData(InteriorSaveData saveData)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["info"] = JsonConvert.SerializeObject(saveData)
        };
        ApiResponse<InteriorSaveData[]> response = await RestApiService.PostAsyncWithToken<ApiResponse<InteriorSaveData[]>>(Endpoints.SaveInteriorUrl, data);
        return response != null && response.Success;
    }

    public static async UniTask<bool> UpdateInteriorData(List<InteriorSaveData> saveData)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["info"] = JsonConvert.SerializeObject(saveData)
        };
        ApiResponse<InteriorSaveData[]> response = await RestApiService.PostAsyncWithToken<ApiResponse<InteriorSaveData[]>>(Endpoints.SaveInteriorsUrl, data);
        return response != null && response.Success;
    }
}
