using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public static class MissionSaveDataDAC
{
    public static async UniTask<ApiResponse<MissionSaveData[]>> GetAllMissionSaveData()
    {
        ApiResponse<MissionSaveData[]> response =
            await RestApiService.GetAsyncWithToken<MissionSaveData[]>(Endpoints.GetMissionsUrl);
        return response;
    }

    public static async UniTask<ApiResponse<bool>> UpdateMissionSaveData(MissionSaveData missionData)
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
