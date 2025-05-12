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
            var response = await RestApiService.GetAsync<string>(Endpoints.RefreshTokenUrl, queryData);
            if(response.ResponseCode != ResponseType.Success) 
                return false;
            TokenManager.LoginToken = response.Data;
            TokenManager.SaveToken(response.Data);
            
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
        var response = await RestApiService.GetAsync<bool>(Endpoints.VerifyTokenUrl, queryData);
        if(response.ResponseCode != ResponseType.Success) return false;
        return response.Data;
    }

    public static async UniTask<TokenData> LoginAsGuestTask()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("uuid", GuestLoginManager.UUID);
        var response =  await RestApiService.GetAsync<TokenData>(Endpoints.GuestLoginUrl, payload);
        if(response.ResponseCode != ResponseType.Success) return null;
        return response.Data;
    }

    public static async UniTask<bool> RegisterAsGuestTask()
    {
        try
        {
            var response = await RestApiService.PostAsync<string>(Endpoints.GuestRegisterUrl);
            if(response.ResponseCode != ResponseType.Success) return false;
            GuestLoginManager.SaveUUID(response.Data);
            
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
        CurrencyData money = new CurrencyData(CurrencyType.Money, 0);
        CurrencyData adTicket = new CurrencyData(CurrencyType.AdTicket, 0);
        List<CurrencyData> currencyDataList = new List<CurrencyData>() { gold, money, adTicket };
        return currencyDataList;
    }
}