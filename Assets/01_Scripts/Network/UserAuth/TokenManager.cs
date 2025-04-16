using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Directory = System.IO.Directory;

public class TokenData
{ 
    public TokenData(string token, string refreshToken)
    {
        this.token = token;
        this.refreshToken = refreshToken;
    }
    public string token;
    public string refreshToken;
}

public static class TokenManager
{
    private static readonly string TokenFileName = "token.txt";
    private static readonly string TokenSaveDirectory = $"{Application.persistentDataPath}/Auth";
    public static string LoginToken;
    public static string RefreshToken;
    
    private static JsonSerializerSettings settings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.All,
    };

    public static void SetToken(string token, string refreshToken)
    {
        LoginToken = token;
        RefreshToken = refreshToken;
        Debug.Log("Token : " + LoginToken);
        Debug.Log("Refresh Token : " + RefreshToken);
    }
    
    
    public static bool ReadToken()
    {
        if(!Directory.Exists(TokenSaveDirectory) || !File.Exists(Path.Combine(TokenSaveDirectory, TokenFileName)))
            return false;
        var path = Path.Combine(TokenSaveDirectory, TokenFileName);
        var data = File.ReadAllText(path);
        var tokenData = JsonConvert.DeserializeObject<TokenData>(data, settings);
            
        SetToken(tokenData.token, tokenData.refreshToken);
        
        return true;
    }

    public static void SaveToken(string token)
    {
        SaveToken(token, RefreshToken);
    }
    
    public static void SaveToken(string token, string refreshToken)
    {
        if(!Directory.Exists(TokenSaveDirectory))
            Directory.CreateDirectory(TokenSaveDirectory);
        
        TokenData data = new TokenData(token, refreshToken);
        
        var path = Path.Combine(TokenSaveDirectory, TokenFileName);
        var json = JsonConvert.SerializeObject(data, settings);

        File.WriteAllText(path, json);
        
        SetToken(token, refreshToken);
    }

    public static void DeleteToken()
    {
        if(!Directory.Exists(TokenSaveDirectory))
            return;
        File.Delete(Path.Combine(TokenSaveDirectory, TokenFileName));
        File.Delete(Path.Combine(TokenSaveDirectory, "guestID.txt"));
        
        SetToken(null, null);
    }
}
