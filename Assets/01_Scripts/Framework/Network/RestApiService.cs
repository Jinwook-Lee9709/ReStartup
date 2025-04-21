using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public static class RestApiService
{
    private static readonly int DefaultTimeout = 5;
    private static readonly int RetryCount = 3;
    
    
    public static UnityWebRequest CreateGetRequest(string url, Dictionary<string, string> data = null)
    {
        url += "?" + GetQueryString(data);
        var request = new UnityWebRequest(url, "GET");

        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new BypassCertificateHandler();
        return request;
    }

    public static string GetQueryString(Dictionary<string, string> data)
    {
        if (data == null || data.Count == 0)
            return string.Empty;
        return string.Join("&", data.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value.ToString())}"));
    }

    public static UnityWebRequest CreatePostRequest(string url, Dictionary<string, string> data = null)
    {
        var request = new UnityWebRequest(url, "POST");
        if (data != null)
        {
            var jsonPayLoad = JsonConvert.SerializeObject(data);
            var bodyRaw = Encoding.UTF8.GetBytes(jsonPayLoad);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

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

    public static async UniTask<ApiResponse<T>> GetAsync<T>(string url,  Dictionary<string, string> data = null)
    {
        using (var request = CreateGetRequest(url, data))
        {
            return await SendRequestAsync<T>(request);
        }
    }

    public static async UniTask<ApiResponse<T>> GetAsyncWithToken<T>(string url, Dictionary<string, string> data = null)
    {
        using (var request = CreateGetRequest(url, data))
        {
            try
            {
                request.SetRequestHeader("Authorization", $"Bearer {TokenManager.LoginToken}");
                return await SendRequestAsync<T>(request);
            }catch(Exception e)
            {
                Debug.Log(e);
                throw e;
            }
        }
    }
    public static async UniTask<ApiResponse<T>> PostAsync<T>(string url, Dictionary<string, string> data = null)
    {
        using (var request = CreatePostRequest(url, data))
        {
            return await SendRequestAsync<T>(request);
        }
    }

    public static async UniTask<ApiResponse<T>> PostAsync<T>(string url, string jsonPayLoad)
    {
        using (var request = CreatePostRequest(url, jsonPayLoad))
        {
            try
            {
                return await SendRequestAsync<T>(request);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw e;
            }
        }
    }
    
    public static async UniTask<ApiResponse<T>> PostAsyncWithToken<T>(string url, Dictionary<string, string> data = null)
    {
        using (var request = CreatePostRequest(url, data))
        {
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.LoginToken}");
            return await SendRequestAsync<T>(request);
        }
    }
    
    public static async UniTask<ApiResponse<T>> SendRequestAsync<T>(UnityWebRequest request)
    {
        request.timeout = DefaultTimeout;
        
        if (!CheckInternetConnection<T>())
        {
            return new ApiResponse<T>(ResponseType.InternetDisconnected, default);
        }
            
        using (request)
        {
            try
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        var data = JsonConvert.DeserializeObject<ApiResponse<T>>(request.downloadHandler.text, settings);
                        return data;
                    }
                    catch (JsonException ex)
                    {
                        Debug.Log(ex);
                        throw new Exception($"JSON Deserialization Error: {ex.Message}");
                    }
                }
                throw new Exception($"Request failed with error: {request.error}, ResponseCode: {request.responseCode}");
            }
            catch (Exception e)
            {
                if (request.responseCode == 401)
                {
                    bool refreshResult = await UserAuthController.RefreshToken();
                    if (refreshResult)
                    {
                        var newRequest = CloneRequest(request);
                        return await SendRequestAsync<T>(newRequest);
                    }
                    var title = LZString.GetUIString("TokenExpiredAlertTitle");
                    var message = LZString.GetUIString("TokenExpiredAlertDescription");
                    ServiceLocator.Instance.GetGlobalService<AlertPopup>().PopUp(title, message);
                    return new ApiResponse<T>(ResponseType.InvalidToken, default);
                }
                else
                {
                    var retryResult = await RetryRequest<T>(request);
                    if (retryResult.ResponseCode == ResponseType.Success)
                    {
                        return retryResult;
                    }
                    
                    var title = LZString.GetUIString("ServerConnectionFailureAlertTitle");
                    var message = LZString.GetUIString("ServerConnectionFailureAlertDescription");
                    ServiceLocator.Instance.GetGlobalService<AlertPopup>().PopUp(title, message);
                    return new ApiResponse<T>(retryResult.ResponseCode, default);
                }
            }
            finally
            {
                if (request != null)
                {
                    request.Dispose();
                }
            }
        }
    }

    private static bool CheckInternetConnection<T>()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            var title = LZString.GetUIString("NetworkFailureAlertTitle");
            var message = LZString.GetUIString("NetworkFailureAlertDescription");
            ServiceLocator.Instance.GetGlobalService<AlertPopup>().PopUp(title, message);
            return false;
        }
        return true;
    }

    private static async UniTask<ApiResponse<T>> RetryRequest<T>(UnityWebRequest request)
    {
        for (int i = 0; i < RetryCount; i++)
        {
            var requestClone = CloneRequest(request);
            using (requestClone)
            {
                try
                {
                    await requestClone.SendWebRequest();
                    if (requestClone.result == UnityWebRequest.Result.Success)
                    {
                        try
                        {
                            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                            var data = JsonConvert.DeserializeObject<ApiResponse<T>>(request.downloadHandler.text, settings);
                            return data;
                        }
                        catch (JsonException ex)
                        {
                            return new ApiResponse<T>(ResponseType.JsonParseError, default);
                        }
                    }
                    throw new Exception($"Request failed with error: {request.error}, ResponseCode: {request.responseCode}");
                }
                catch(Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
        return new ApiResponse<T>(ResponseType.Timeout, default);
    }

    private static UnityWebRequest CloneRequest(UnityWebRequest originalRequest)
    {
        var newRequest = new UnityWebRequest(originalRequest.url, originalRequest.method)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        newRequest.timeout = DefaultTimeout;
        newRequest.SetRequestHeader("Authorization", $"Bearer {TokenManager.LoginToken}");


        // Clone body (if present in the original POST/PUT request)
        if (originalRequest.uploadHandler != null)
        {
            newRequest.uploadHandler = new UploadHandlerRaw(originalRequest.uploadHandler.data)
            {
                contentType = originalRequest.uploadHandler.contentType
            };
        }
        newRequest.certificateHandler = new BypassCertificateHandler();

        return newRequest;
    }

    private class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // 모든 인증서 신뢰
        }
    }
}