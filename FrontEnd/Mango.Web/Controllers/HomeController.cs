using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Mango.Web.Services.IServices;
using Mango.Web.DTOs;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService,
        ICartService cartService)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto> productList = new();
        var token = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetAllProductsAsync<ResponseDto>(token);
        if (response?.IsSuccess ?? false)
        {
            productList = JSONHelper.Deserialize<List<ProductDto>>(response.Result);
        }
        return View(productList);
    }

    [Authorize]
    public async Task<IActionResult> Details(int productId)
    {
        ProductDto product = new();
        var token = await HttpContext.GetTokenAsync("access_token");
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, token);
        if (response?.IsSuccess ?? false)
        {
            product = JSONHelper.Deserialize<ProductDto>(response.Result);
        }
        return View(product);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Details(ProductDto productDto)
    {
        var cartDto = new CartDto
        {
            CartHeader = new CartHeaderDto
            {
                UserId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value,
            }
        };

        var cartDetailDto = new CartDetailDto
        {
            Count = productDto.Count,
            ProductId = productDto.Id
        };

        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var resp = await _productService.GetProductByIdAsync<ResponseDto>(productDto.Id, accessToken);
        if (resp?.IsSuccess ?? false)
        {
            cartDetailDto.Product = JSONHelper.Deserialize<ProductDto>(resp.Result.ToString());
        }

        cartDto.CartDetails = new List<CartDetailDto>
        {
            cartDetailDto
        };

        var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);
        if (addToCartResp?.IsSuccess ?? false)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(productDto);
    }

    [Authorize]
    public async Task<IActionResult> Login()
    {
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Logout()
    {
        return SignOut("Cookies", "oidc");
    }
}
