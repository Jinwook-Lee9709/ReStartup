using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RankerDataDAC
{
    public static async UniTask<ApiResponse<RankerData[]>> GetRankerData()
    {
        ApiResponse<RankerData[]> response = await RestApiService.GetAsync<RankerData[]>(Endpoints.GetRankerUrl);
        return response;
    }

    public static async UniTask<ApiResponse<UserRankData>> GetUserRank()
    {
        ApiResponse<UserRankData> response = await RestApiService.GetAsyncWithToken<UserRankData>(Endpoints.GetUserRankUrl);
        return response;
    }
    
    
}
