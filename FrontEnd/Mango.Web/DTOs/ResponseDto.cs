using static Mango.Web.SD;

namespace Mango.Web.DTOs;

public class ResponseDto
{
    public bool IsSuccess { get; set; }
    public object Result { get; set; }
    public string DisplayMessage { get; set; }
    public List<string> ErrorMessages { get; set; }
}

public class ApiRequest
{
    public ApiType ApiType { get; set; } = ApiType.GET;
    public string Url { get; set; }
    public object Data { get; set; }
    public string AccessToken { get; set; }
}