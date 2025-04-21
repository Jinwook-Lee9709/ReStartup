using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class UserDataDAC
{
    public static async Task<UserData> GetUserData(string uid)
    {
        var result = await RestApiService.GetAsync<UserData>($"https://localhost:443/api/getInfo?uuid={uid}");
        return result.Data;
    }
    
    
    
}