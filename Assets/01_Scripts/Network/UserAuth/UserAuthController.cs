using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Directory = System.IO.Directory;

public static class UserAuthController
{
    public static async UniTask<bool> RefreshToken()
    {
        if(TokenManager.RefreshToken == null) return false;
        try
        {   
            Dictionary<string, string> queryData = new Dictionary<string, string>
            {
                { "token", TokenManager.RefreshToken }
            };
            string newToken = await RestApiService.GetAsync<string>(Endpoints.RefreshTokenUrl, queryData);
            TokenManager.LoginToken = newToken;
            TokenManager.SaveToken(newToken);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public static async UniTask<bool> VerifyToken()
    {
        if(TokenManager.LoginToken == null) return false;
        Dictionary<string, string> queryData = new Dictionary<string, string>
        {
            { "token", TokenManager.LoginToken }
        };
        bool isValid = await RestApiService.GetAsync<bool>(Endpoints.VerifyTokenUrl, queryData);
        return isValid;
    }

    public static async UniTask<TokenData> LoginAsGuestTask()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("uuid", GuestLoginManager.UUID);
        return await RestApiService.GetAsync<TokenData>(Endpoints.GuestLoginUrl, payload);
    }

    public static async UniTask<bool> RegisterAsGuestTask()
    {
        try
        {
            var uuid = await RestApiService.PostAsync<string>(Endpoints.GuestRegisterUrl);
            GuestLoginManager.SaveUUID(uuid);
            
            var token = await LoginAsGuestTask();
            TokenManager.SaveToken(token.token, token.refreshToken);
            
            var currencyDataList = CreateInitialCurrencyData();
            return await CurrencyDataDAC.UpdateCurrencyData(currencyDataList);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }
    
    private static List<CurrencyData> CreateInitialCurrencyData()
    {
        CurrencyData gold = new CurrencyData(CurrencyType.Gold, 0);
        //CurrencyData money = new CurrencyData(CurrencyType.Money, 0);
        CurrencyData money = new CurrencyData(CurrencyType.Money, 1000000);
        List<CurrencyData> currencyDataList = new List<CurrencyData>() { gold, money };
        return currencyDataList;
    }
}