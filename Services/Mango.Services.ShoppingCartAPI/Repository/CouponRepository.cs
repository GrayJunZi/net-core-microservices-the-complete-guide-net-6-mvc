using Mango.Services.ShoppingCartAPI.Common;
using Mango.Services.ShoppingCartAPI.DTOs;

namespace Mango.Services.ShoppingCartAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient _httpClient;

    public CouponRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CouponAPI");
    }

    public async Task<CouponDto> GetCoupon(string couponCode)
    {
        var response = await _httpClient.GetAsync($"/api/coupon/{couponCode}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonHelper.Deserialize<ResponseDto>(apiContent);
        if (resp.IsSuccess)
        {
            return JsonHelper.Deserialize<CouponDto>(resp.Result);
        }
        return new CouponDto();
    }
}