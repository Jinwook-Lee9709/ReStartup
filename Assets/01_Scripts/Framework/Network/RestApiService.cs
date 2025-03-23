using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class RestApiService
{
    public static UnityWebRequest CreateGetRequest(string url)
    {
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        string jsonPayload;

        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new BypassCertificateHandler();
        return request;
    }

    public static UnityWebRequest CreatePostRequest(string url, List<KeyValuePair<string, string>> data)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        string jsonPayLoad = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayLoad);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificateHandler();
        return request;
    }

    public static async Task<T> GetAsync<T>(string url)
    {
        using (UnityWebRequest request = CreateGetRequest(url))
        {
            return await SendRequestAsync<T>(request);
        }
    }

    public static async Task<T> PostAsync<T>(string url, List<KeyValuePair<string, string>> data)
    {
        using (UnityWebRequest request = CreatePostRequest(url, data))
        {
            return await SendRequestAsync<T>(request);
        }
    }

    public static async Task<T> SendRequestAsync<T>(UnityWebRequest request)
    {
        using (request)
        {
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
        
            if (request.result == UnityWebRequest.Result.Success)
            {
                
                Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(request.downloadHandler.text, settings);
            }
            else
            {
                throw new Exception($"Request failed: {request.error}");
            }
        }
    }
    
    private class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // 모든 인증서 신뢰
        }
    }
}
