using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class PromotionDataDAC
{
    public static async UniTask<ApiResponse<PromotionData[]>> GetPromotionData()
    {
        var data = await RestApiService.GetAsyncWithToken<PromotionData[]>(Endpoints.GetPromotionsUrl);
        return data;
    }

    public static async UniTask<bool> UpdatePromotionData(List<PromotionData> promotions)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            ["info"] = JsonConvert.SerializeObject(promotions)
        };
        ApiResponse<PromotionData[]> response = await RestApiService.PostAsyncWithToken<PromotionData[]>(Endpoints.SavePromotionsUrl, data);
        return response != null && response.ResponseCode == ResponseType.Success;
    }
}
