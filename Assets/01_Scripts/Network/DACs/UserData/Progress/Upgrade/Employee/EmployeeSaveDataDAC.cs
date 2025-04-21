using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class EmployeeSaveDataDAC
{
    public static async UniTask<ApiResponse<EmployeeSaveData[]>> GetEmployeeData(int themeId)
    {
        var payload = new Dictionary<string, string>();
        payload.Add("theme", themeId.ToString());
        var data = await RestApiService.GetAsyncWithToken<EmployeeSaveData[]>(Endpoints.GetEmployeeByTheme, payload);
        return data;
    }
    
    public static async UniTask<bool> UpdateEmployeeData(EmployeeSaveData saveData)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["info"] = JsonConvert.SerializeObject(saveData)
        };
        ApiResponse<EmployeeSaveData[]> response =
            await RestApiService.PostAsyncWithToken<EmployeeSaveData[]>(Endpoints.SaveEmployeeUrl, data);
        return response != null && response.ResponseCode == ResponseType.Success;
    }

    public static async UniTask<bool> UpdateEmployeeData(List<EmployeeSaveData> saveData)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            ["info"] = JsonConvert.SerializeObject(saveData)
        };
        ApiResponse<EmployeeSaveData[]> response = await RestApiService.PostAsyncWithToken<EmployeeSaveData[]>(Endpoints.SaveEmployeesUrl, data);
        return response != null && response.ResponseCode == ResponseType.Success;
    }
    
    
}
