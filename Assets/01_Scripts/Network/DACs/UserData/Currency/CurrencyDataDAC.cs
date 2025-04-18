using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class CurrencyDataDAC
{
    public static async UniTask<ApiResponse<CurrencyData[]>> GetCurrencyData()
    {
        var data = await RestApiService.GetAsyncWithToken<ApiResponse<CurrencyData[]>>(Endpoints.GetAllCurrenciesUrl);
        return data;
    }

    public static async UniTask<bool> UpdateCurrencyData(List<CurrencyData> currencies)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["currencies"] = JsonConvert.SerializeObject(currencies);
        ApiResponse<CurrencyData[]> response = await RestApiService.PostAsyncWithToken<ApiResponse<CurrencyData[]>>(Endpoints.SaveCurrenciesUrl, data);
        return response != null && response.Success;
    }
}
