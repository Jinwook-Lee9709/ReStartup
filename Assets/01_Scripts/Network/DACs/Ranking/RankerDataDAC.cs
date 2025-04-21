using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RankerDataDAC
{
    public static async UniTask<ApiResponse<RankerData[]>> GetRankerData()
    {
        ApiResponse<RankerData[]> response = await RestApiService.GetAsync<RankerData[]>(Endpoints.GetBuffsUrl);
        return response;
    }
}
