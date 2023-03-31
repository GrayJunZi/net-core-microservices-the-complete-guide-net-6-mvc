using Mango.Web.DTOs;
using Mango.Web.Services.IServices;
using System.Text;
using System.Text.Json;

namespace Mango.Web.Services;
public class BaseService : IBaseService
{
    public ResponseDto response { get; set; }
    public IHttpClientFactory _httpClientFactory { get; set; }
    public BaseService(IHttpClientFactory httpClientFactory)
    {
        response = new ResponseDto();
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MangoAPI");
            client.DefaultRequestHeaders.Clear();

            var message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.Headers.Add("Content-Type", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            if (apiRequest.Data != null)
            {
                message.Content = new StringContent(JsonSerializer.Serialize(apiRequest.Data), Encoding.UTF8, "application/json");
            }

            switch (apiRequest.ApiType)
            {
                case SD.ApiType.GET:
                    message.Method = HttpMethod.Get;
                    break;
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            var apiResponse = await client.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonSerializer.Deserialize<T>(apiContent);

            return apiResponseDto;
        }
        catch (Exception ex)
        {
            var responseDto = new ResponseDto
            {
                DisplayMessage = "Error",
                ErrorMessages = new List<string> { ex.ToString() },
                IsSuccess = false,
            };

            var content = JsonSerializer.Serialize(responseDto);
            var apiResponseDto = JsonSerializer.Deserialize<T>(content);
            return apiResponseDto;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(true);
    }
}
