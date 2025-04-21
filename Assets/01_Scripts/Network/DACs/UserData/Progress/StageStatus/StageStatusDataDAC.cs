using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public static class StageStatusDataDAC
{
    public static async UniTask<ApiResponse<StageStatusData[]>> GetStageStatusData()
    {
        var data = await RestApiService.GetAsyncWithToken<StageStatusData[]>(Endpoints.GetAllStageStatusUrl);
        return data;
    }

    public static async UniTask<bool> UpdateStageStatusData(StageStatusData stageStatus)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["info"] = JsonConvert.SerializeObject(stageStatus)
        };
        ApiResponse<StageStatusData[]> response = await RestApiService.PostAsyncWithToken<StageStatusData[]>(Endpoints.SaveStageStatusUrl, data);
        return response != null && response.ResponseCode == ResponseType.Success;
    }
}
