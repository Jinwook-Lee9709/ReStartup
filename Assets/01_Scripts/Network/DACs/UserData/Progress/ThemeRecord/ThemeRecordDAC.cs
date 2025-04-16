using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class ThemeRecordDAC
{
    public static async UniTask<ApiResponse<ThemeRecordData[]>> ThemeRecordData(int themeId)
    {
        var payload = new Dictionary<string, string>();
        payload.Add("theme", themeId.ToString());
        var result = await RestApiService.GetAsyncWithToken<ApiResponse<ThemeRecordData[]>>(Endpoints.GetThemeRecordsUrl, payload);
        return result;
    }
}
