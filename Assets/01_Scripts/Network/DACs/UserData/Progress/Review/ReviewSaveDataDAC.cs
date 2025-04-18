using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ReviewSaveDataDAC
{
    public static async UniTask<ApiResponse<ReviewSaveData[]>> GetReviewSaveData()
    {
        ApiResponse<ReviewSaveData[]> response =
            await RestApiService.GetAsyncWithToken<ApiResponse<ReviewSaveData[]>>(Endpoints.GetAllReivewUrl);
        return response;
    }

    public static async UniTask<bool> InsertReviewData(bool isPositive, int id, DateTime createdTime)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            ["isPositive"] = (isPositive ? 1 : 0).ToString(),
            ["reviewId"] = id.ToString(),
            ["createdTime"] = JsonConvert.SerializeObject(createdTime)
        };
        ApiResponse<ReviewSaveData> response =
            await RestApiService.PostAsyncWithToken<ApiResponse<ReviewSaveData>>(Endpoints.InsertReivewUrl, payload);
        return response != null && response.Success;
    }

    public static async UniTask<bool> DeleteReviewData(int index)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            ["orderIndex"] = index.ToString()
        };
        ApiResponse<ReviewSaveData> response =
            await RestApiService.PostAsyncWithToken<ApiResponse<ReviewSaveData>>(Endpoints.DeleteReivewUrl, payload);
        return response != null && response.Success;
    }
}
