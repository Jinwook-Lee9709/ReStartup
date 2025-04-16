using System;

[Serializable]
public class ApiResponse<T>
{
    public bool Success { get; set; } // 요청 성공 여부
    public T Data { get; set; }       // 실제 데이터

    public ApiResponse(bool success, T data)
    {
        Success = success;
        Data = data;
    }
}