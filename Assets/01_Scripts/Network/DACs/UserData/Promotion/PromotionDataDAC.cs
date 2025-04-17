using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class PromotionDataDAC
{
    public static async UniTask<ApiResponse<PromotionData[]>> GetPromotionData()
    {
        var data = await RestApiService.GetAsyncWithToken<ApiResponse<PromotionData[]>>(Endpoints.GetPromotionsUrl);
        return data;
    }

    public static async UniTask<bool> UpdatePromotionData(List<PromotionData> promotions)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            ["info"] = JsonConvert.SerializeObject(promotions)
        };
        ApiResponse<PromotionData[]> response = await RestApiService.PostAsyncWithToken<ApiResponse<PromotionData[]>>(Endpoints.SavePromotionsUrl, data);
        return response != null && response.Success;
    }
}
