using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

public static class RestApiService
{
    public static UnityWebRequest CreateGetRequest(string url)
    {
        var request = new UnityWebRequest(url, "GET");
        string jsonPayload;

        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new BypassCertificateHandler();
        return request;
    }

    public static UnityWebRequest CreatePostRequest(string url, List<KeyValuePair<string, string>> data)
    {
        var request = new UnityWebRequest(url, "POST");
        var jsonPayLoad = JsonConvert.SerializeObject(data);
        var bodyRaw = Encoding.UTF8.GetBytes(jsonPayLoad);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificateHandler();
        return request;
    }

    public static UnityWebRequest CreatePostRequest(string url, string jsonPayLoad)
    {
        var request = new UnityWebRequest(url, "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(jsonPayLoad);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new BypassCertificateHandler();
        return request;
    }

    public static async Task<T> GetAsync<T>(string url)
    {
        using (var request = CreateGetRequest(url))
        {
            return await SendRequestAsync<T>(request);
        }
    }

    public static async Task<T> PostAsync<T>(string url, List<KeyValuePair<string, string>> data)
    {
        using (var request = CreatePostRequest(url, data))
        {
            return await SendRequestAsync<T>(request);
        }
    }

    public static async Task<T> PostAsync<T>(string url, string jsonPayLoad)
    {
        using (var request = CreatePostRequest(url, jsonPayLoad))
        {
            return await SendRequestAsync<T>(request);
        }
    }


    public static async Task<T> SendRequestAsync<T>(UnityWebRequest request)
    {
        using (request)
        {
            request.SendWebRequest();
            while (!request.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                return JsonConvert.DeserializeObject<T>(request.downloadHandler.text, settings);
            }

            throw new Exception($"Request failed: {request.error}");
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