using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderAPI.Models;

public class OrderHeader
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string CouponCode { get; set; }
    public decimal OrderTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public string Name { get; set; }
    public DateTime PickupDateTime { get; set; }
    public DateTime OrderTime { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string CardNumber { get; set; }
    public string CVV { get; set; }
    public string ExpiryMonthYear { get; set; }
    public int CartTotalItems { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
    public bool PaymentStatus { get; set; }
}
