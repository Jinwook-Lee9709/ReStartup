
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class BuffSaveDataDAC
{
    public static async UniTask<ApiResponse<BuffSaveData[]>>GetAllBuffSaveData()
    {
        ApiResponse<BuffSaveData[]> response = await RestApiService.GetAsyncWithToken<BuffSaveData[]>(Endpoints.GetBuffsUrl);
        return response;
    }
    
    public static async UniTask<bool> UpdateBuffSaveData(BuffSaveData buffSaveData)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            ["id"] = buffSaveData.id.ToString(),
            ["buff_type"] = JsonConvert.SerializeObject(buffSaveData.type),
            ["remain_time"] = buffSaveData.remainTime.ToString()
        };
        ApiResponse<BuffSaveData> response = await RestApiService.PostAsyncWithToken<BuffSaveData>(Endpoints.SaveBuffUrl, payload);
        return response != null && response.ResponseCode == ResponseType.Success;
    }

    public static async UniTask<bool> UpdateBuffSaveData(List<BuffSaveData> buffSaveData)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            ["info"] = JsonConvert.SerializeObject(buffSaveData)
        };
        ApiResponse<BuffSaveData> response = await RestApiService.PostAsyncWithToken<BuffSaveData>(Endpoints.SaveBuffsUrl, payload);
        return response != null && response.ResponseCode == ResponseType.Success;
    }

    public static async UniTask<bool> DeleteBuffSaveData(int id)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            ["id"] = id.ToString(),
        };
        ApiResponse<BuffSaveData> response = await RestApiService.PostAsyncWithToken<BuffSaveData>(Endpoints.DeleteBuffUrl, payload);
        return response != null && response.ResponseCode == ResponseType.Success;
    }
}
