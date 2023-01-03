namespace ElmaSmartFarm.ApiClient.Models;

public class HttpResponseBaseModel
{
    public int StatusCode { get; set; }
    public bool IsSuccess => StatusCode >= 200 || StatusCode <= 299;
}

public class HttpResponseModel<T> : HttpResponseBaseModel
{
    public T Payload { get; set; }
}