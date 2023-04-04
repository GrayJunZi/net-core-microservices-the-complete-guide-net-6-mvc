namespace Mango.Services.OrderAPI.Messages;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; }
    public IEnumerable<CartDetailDto> CartDetails { get; set; }
}