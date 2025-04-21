using System;

[Serializable]
public class ApiResponse<T>
{
    public ResponseType ResponseCode { get; set; } // 요청 성공 여부
    public T Data { get; set; }       // 실제 데이터

    public ApiResponse(ResponseType responseCode, T data)
    {
        ResponseCode = responseCode;
        Data = data;
    }
}