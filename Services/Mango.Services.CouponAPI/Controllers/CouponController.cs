using Azure;
using Mango.Services.CouponAPI.DTOs;
using Mango.Services.CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponController : ControllerBase
{
    private readonly ICouponRepository _couponRepository;
    protected ResponseDto _response;

    public CouponController(ICouponRepository couponRepository)
	{
        _couponRepository = couponRepository;
        _response = new ResponseDto();
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetCoupon(string code)
    {
        try
        {
            var couponDto = await _couponRepository.GetCouponByCode(code);
            _response.IsSuccess = true;
            _response.Result = couponDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }
}
