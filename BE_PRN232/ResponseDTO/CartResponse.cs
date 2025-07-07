using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class CartResponse
{
    public CartResponse(Cart cart,string baseUrl)
    {
        CartId = cart.CartId;
        UserId = cart.UserId;
        CreatedAt = cart.CreatedAt;
        UpdatedAt = cart.UpdatedAt;
        var cartItems = cart.CartItems;
        CartItems = cartItems.Select(c=>new CartItemResponse(c,baseUrl)).ToList();
        Total = cartItems.Sum(c=>c.Quantity);
    }
    public int CartId { get; set; }

    public Guid UserId { get; set; }

    public int Total { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    public List<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();
}