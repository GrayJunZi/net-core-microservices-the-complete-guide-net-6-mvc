namespace Mango.Web.DTOs;

public class CartHeaderDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string CouponCode { get; set; }
    public double OrderTotal { get; set; }
}
