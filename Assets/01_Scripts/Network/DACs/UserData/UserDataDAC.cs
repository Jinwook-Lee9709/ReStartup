using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public static class UserDataDAC
{
    public static async UniTask<ApiResponse<string>> GetUserName()
    {
        ApiResponse<string> response = await RestApiService.GetAsyncWithToken<string>(Endpoints.GetUserName);
        return response;
    }
    
    public static async UniTask<ApiResponse<string>> SaveUserName()
    {
        ApiResponse<string> response = await RestApiService.PostAsyncWithToken<string>(Endpoints.GetUserName);
        return response;
    }
}