using Mango.Web.Common;
using Mango.Web.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public CartController(
        IProductService productService,
        ICartService cartService,
        ICouponService couponService)
    {
        _productService = productService;
        _cartService = cartService;
        _couponService = couponService;
    }
    public async Task<IActionResult> Index()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    public async Task<IActionResult> Remove(int cartDetailId)
    {
        var userId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailId, accessToken);

        if (response?.IsSuccess ?? false)
        {
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        var userId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);

        if (response?.IsSuccess ?? false)
        {
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        var userId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.RemoveCouponAsync<ResponseDto>(cartDto.CartHeader.UserId, accessToken);

        if (response?.IsSuccess ?? false)
        {
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CartDto cartDto)
    {
        try
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.CheckoutAsync<ResponseDto>(cartDto.CartHeader, accessToken);
            if (!response?.IsSuccess ?? true)
            {
                ViewBag.Error = response.DisplayMessage;
                return RedirectToAction(nameof(Checkout));
            }
            return RedirectToAction(nameof(Confirmation));
        }
        catch (Exception ex)
        {
            return View(cartDto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation()
    {
        return View();
    }

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

        CartDto cartDto = null;
        if (response?.IsSuccess ?? false)
        {
            cartDto = JsonHelper.Deserialize<CartDto>(response.Result.ToString());
        }

        if (cartDto?.CartHeader != null)
        {
            if (!string.IsNullOrWhiteSpace(cartDto.CartHeader.CouponCode))
            {
                var couponResponse = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode, accessToken);
                if (couponResponse?.IsSuccess ?? false)
                {
                    var coupon = JSONHelper.Deserialize<CouponDto>(couponResponse.Result.ToString());
                    cartDto.CartHeader.DiscountTotal = coupon.CouponAmount;
                }
            }

            foreach (var detail in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
            }

            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
        }
        return cartDto;
    }
}
