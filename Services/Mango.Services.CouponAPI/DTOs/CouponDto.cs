namespace Mango.Services.CouponAPI.DTOs;

public class CouponDto
{
    public int Id { get; set; }
    public string CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
}
