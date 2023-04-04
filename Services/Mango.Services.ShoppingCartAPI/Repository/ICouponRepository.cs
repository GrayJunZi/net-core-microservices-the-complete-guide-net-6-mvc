using Mango.Services.ShoppingCartAPI.DTOs;

namespace Mango.Services.ShoppingCartAPI.Repository;

public interface ICouponRepository
{
    Task<CouponDto> GetCoupon(string couponCode);
}
