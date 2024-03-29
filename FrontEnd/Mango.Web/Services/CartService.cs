﻿using Mango.Web.DTOs;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class CartService : BaseService, ICartService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CartService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId,
            AccessToken = token
        });
    }
    public async Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/AddCart",
            AccessToken = token
        });
    }
    public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/UpdateCart",
            AccessToken = token
        });
    }
    public async Task<T> RemoveFromCartAsync<T>(int cartId, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart/" + cartId,
            AccessToken = token
        });
    }

    public async Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
            AccessToken = token
        });
    }

    public async Task<T> RemoveCouponAsync<T>(string userId, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCoupon/" + userId,
            AccessToken = token
        });
    }

    public async Task<T> CheckoutAsync<T>(CartHeaderDto cartHeaderDto, string token = null)
    {
        return await this.SendAsync<T>(new ApiRequest
        {
            ApiType = SD.ApiType.POST,
            Data = cartHeaderDto,
            Url = SD.ShoppingCartAPIBase + "/api/cart/cehckout/",
            AccessToken = token
        });
    }
}
