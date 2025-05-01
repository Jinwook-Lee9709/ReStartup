using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class ThemeRecordDAC
{
    public static async UniTask<ApiResponse<ThemeRecordData[]>>GetThemeRecordData(int themeId)
    {
        var payload = new Dictionary<string, string> { { "theme", themeId.ToString() } };
        var result = await RestApiService.GetAsyncWithToken<ThemeRecordData[]>(Endpoints.GetThemeRecordsUrl, payload);
        return result;
    }
    

    public static async UniTask<bool> UpdateThemeRecordData(ThemeRecordData saveData)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["records"] = JsonConvert.SerializeObject(saveData)
        };
        ApiResponse<ThemeRecordData[]> response = await RestApiService.PostAsyncWithToken<ThemeRecordData[]>(Endpoints.InsertThemeRecordsUrl, data);
        return response != null && response.ResponseCode == ResponseType.Success;
    }

    public static async UniTask<bool> UpdateThemeRankpoint(int themeId, int rankpoint)
    {
        Dictionary<string, string> payload = new()
        {
            ["theme"] = themeId.ToString(),
            ["rank_point"] = rankpoint.ToString()
        };
        ApiResponse<ThemeRecordData> response = await RestApiService.PostAsyncWithToken<ThemeRecordData>(Endpoints.SaveRankPointUrl, payload);
        return response != null && response.ResponseCode == ResponseType.Success;
    }

    public static async UniTask<bool> UpdateThemeRank(int themeId, int rank)
    {
        Dictionary<string, string> payload = new()
        {
            ["theme"] = themeId.ToString(),
            ["ranking"] = rank.ToString()
        };
        ApiResponse<ThemeRecordData> response = await RestApiService.PostAsyncWithToken<ThemeRecordData>(Endpoints.SaveRankingUrl, payload);
        return response != null && response.ResponseCode == ResponseType.Success;
    }
}
