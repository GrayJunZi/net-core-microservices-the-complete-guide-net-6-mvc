using System.Text.Json;

namespace Mango.Services.PaymentAPI.Common;

public static class JsonHelper
{
    public static T Deserialize<T>(object json)
    {
        return JsonSerializer.Deserialize<T>(json.ToString(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }
}