using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public static class MissionSaveDataDAC
{
    public static async UniTask<ApiResponse<MissionData[]>> GetAllMissionSaveData()
    {
        ApiResponse<MissionData[]> response =
            await RestApiService.GetAsyncWithToken<MissionData[]>(Endpoints.GetMissionsUrl);
        return response;
    }

    public static async UniTask<ApiResponse<bool>> UpdateMissionSaveData(MissionData missionData)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            ["info"] = JsonConvert.SerializeObject(missionData)
        };
        ApiResponse<bool> response =
            await RestApiService.PostAsyncWithToken<bool>(Endpoints.SaveMissionUrl, payload);
        return response;
    }
    
}
