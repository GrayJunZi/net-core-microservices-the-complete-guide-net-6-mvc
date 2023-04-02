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

    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
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
