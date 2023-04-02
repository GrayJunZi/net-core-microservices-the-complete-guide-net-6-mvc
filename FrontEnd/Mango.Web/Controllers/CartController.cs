using Mango.Web.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public CartController(
        IProductService productService,
        ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
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

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

        CartDto cartDto = null;
        if (response?.IsSuccess ?? false)
        {
            cartDto = JSONHelper.Deserialize<CartDto>(response.Result.ToString());
        }

        if (cartDto?.CartHeader != null)
        {
            foreach (var detail in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
            }
        }
        return cartDto;
    }
}
