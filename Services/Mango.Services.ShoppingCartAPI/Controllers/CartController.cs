using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.DTOs;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IMessageBus _messageBus;
    protected ResponseDto _response;

    public CartController(
        ICartRepository cartRepository,
        ICouponRepository couponRepository,
        IMessageBus messageBus)
    {
        _cartRepository = cartRepository;
        _couponRepository = couponRepository;
        _messageBus = messageBus;
        _response = new ResponseDto();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        try
        {
            var cartDto = await _cartRepository.GetCartByUserId(userId);
            _response.IsSuccess = true;
            _response.Result = cartDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }

    [HttpPost]
    public async Task<IActionResult> AddCart(CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.CreateUpdateCart(cartDto);
            _response.IsSuccess = true;
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart(CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.CreateUpdateCart(cartDto);
            _response.IsSuccess = true;
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }

    [HttpPost("{cartId}")]
    public async Task<IActionResult> RemoveCart(int cartId)
    {
        try
        {
            var isSuccess = await _cartRepository.RemoveFromCart(cartId);
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

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        try
        {
            var isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
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

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(string userId)
    {
        try
        {
            var isSuccess = await _cartRepository.RemoveCoupon(userId);
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

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutHeaderDto checkoutHeaderDto)
    {
        try
        {
            var cartDto = await _cartRepository.GetCartByUserId(checkoutHeaderDto.UserId);
            if (cartDto == null)
                return BadRequest();

            if (!string.IsNullOrWhiteSpace(checkoutHeaderDto.CouponCode))
            {
                var couponDto = await _couponRepository.GetCoupon(checkoutHeaderDto.CouponCode);
                if (checkoutHeaderDto.DiscountTotal != couponDto.DiscountAmount)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
                    _response.DisplayMessage = "Coupon Price has changed, please confirm";
                    return Ok(_response);
                }
            }

            checkoutHeaderDto.CartDetails = cartDto.CartDetails;

            await _messageBus.PublishMessage(checkoutHeaderDto, "checkoutmessagetopic");

            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }
        return Ok(_response);
    }
}
