using Mango.Web.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var productList = new List<ProductDto>();
        var response = await _productService.GetAllProductsAsync<ResponseDto>();
        if (response?.IsSuccess ?? false)
        {
            productList = Deserialize<List<ProductDto>>(response.Result);
        }
        return View(productList);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.CreateProductAsync<ResponseDto>(productDto);
            if (response?.IsSuccess ?? false)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(productDto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _productService.GetProductByIdAsync<ResponseDto>(id);
        if (response?.IsSuccess ?? false)
        {
            var productDto = Deserialize<ProductDto>(response.Result);
            return View(productDto);
        }
        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.UpdateProductAsync<ResponseDto>(productDto);
            if (response?.IsSuccess ?? false)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(productDto);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _productService.GetProductByIdAsync<ResponseDto>(id);
        if (response?.IsSuccess ?? false)
        {
            var productDto = Deserialize<ProductDto>(response.Result);
            return View(productDto);
        }
        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.DeleteProductAsync<ResponseDto>(productDto.Id);
            if (response?.IsSuccess ?? false)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(productDto);
    }

    private T Deserialize<T>(object json)
    {
        return JsonSerializer.Deserialize<T>(json.ToString(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }
}
