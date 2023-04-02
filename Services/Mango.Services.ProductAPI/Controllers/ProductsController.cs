using Mango.Services.ProductAPI.DTOs;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    protected ResponseDto _response;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        _response = new ResponseDto();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var productList = await _productRepository.GetProducts();
            _response.IsSuccess = true;
            _response.Result = productList;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var product = await _productRepository.GetProductById(id);
            _response.IsSuccess = true;
            _response.Result = product;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductDto productDto)
    {
        try
        {
            var product = await _productRepository.CreateUpdateProduct(productDto);
            _response.IsSuccess = true;
            _response.Result = product;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ProductDto productDto)
    {
        try
        {
            var product = await _productRepository.CreateUpdateProduct(productDto);
            _response.IsSuccess = true;
            _response.Result = product;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var isSuccess = await _productRepository.DeleteProduct(id);
            _response.IsSuccess = true;
            _response.Result = isSuccess;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }
}
