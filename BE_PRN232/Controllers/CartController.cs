using BE_PRN232.Configs;
using BE_PRN232.Entities;
using BE_PRN232.RequestDTO;
using BE_PRN232.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BE_PRN232.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly EcommerceClothingDbContext _context;
    private readonly AppSettings _appSettings;
    public CartController(EcommerceClothingDbContext context, AppSettings appSettings)
    {
        _context = context;
        _appSettings = appSettings;
    }

    [HttpPost("add-to-cart/{userId}")]
    public async Task<IActionResult> AddToCard([FromRoute] string userId, [FromBody] CartItemRequest cartItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId.ToString() == userId);
            if (user == null)
                return Unauthorized("User not found");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = new Guid(userId), CreatedAt = DateTime.Now, CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == cartItem.VariantId);
            if (variant == null)
                return NotFound("Variant not found");

            if (variant.StockQuantity == 0)
                return BadRequest("Variant stock quantity = 0");

            if (cartItem.Quantity > variant.StockQuantity)
                return BadRequest("Quantity cannot be greater than variant quantity");

            var existingItem = cart.CartItems.FirstOrDefault(c => c.VariantId == cartItem.VariantId);
            if (existingItem == null)
            {
                cart.CartItems.Add(new CartItem
                {
                    VariantId = cartItem.VariantId, Quantity = cartItem.Quantity
                });
            }
            else
            {
                existingItem.Quantity += cartItem.Quantity;
                _context.Entry(existingItem).State = EntityState.Modified;
            }

            variant.StockQuantity -= cartItem.Quantity;
            _context.Entry(variant).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok("Added to cart successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("get-cart/{userId}")]
    public async Task<IActionResult> GetCart([FromRoute] string userId)
    {
        try
        {
            var cart = await _context.Carts
                .Include(c=>c.CartItems)
                .ThenInclude(c=>c.Variant)
                .ThenInclude(c=>c.Product)
                .ThenInclude(c=>c.ProductImages)
                .FirstOrDefaultAsync(c=>c.UserId.ToString() == userId);
            if(cart == null)
                return NotFound("Cart not found");
            var response = new CartResponse(cart,_appSettings.BaseUrl);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("delete-cart-items/{cartItemId}")]
    public async Task<IActionResult> DeleteCartItem([FromRoute] int cartItemId)
    {
        try
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Variant)
                .FirstOrDefaultAsync(c => c.CartItemId == cartItemId);
            if (cartItem == null)
                return NotFound("Cart item not found");
            if (cartItem.Variant != null)
            {
                cartItem.Variant.StockQuantity += cartItem.Quantity;
                _context.Entry(cartItem.Variant).State = EntityState.Modified;
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart item deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("update-cart-items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItem(
        [FromRoute] int cartItemId,
        [FromQuery] bool increase = true,
        [FromQuery] int quantity = 1)
    {
        if (quantity <= 0)
        {
            return BadRequest("Quantity cannot be less or equal to zero");
        }
        try
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Variant)
                .FirstOrDefaultAsync(c => c.CartItemId == cartItemId);

            if (cartItem == null)
                return NotFound("Cart item not found");

            var variant = cartItem.Variant;
            if (variant == null)
                return NotFound("Associated product variant not found");

            if (increase)
            {
                if (variant.StockQuantity < quantity)
                    return BadRequest("Not enough stock to increase");

                cartItem.Quantity += quantity;
                variant.StockQuantity -= quantity;
            }
            else
            {
                if (cartItem.Quantity <= 1)
                    return BadRequest("Cannot decrease below 1");

                if (cartItem.Quantity - quantity < 1)
                    return BadRequest("Quantity after decrease cannot be less than 1");

                cartItem.Quantity -= quantity;
                variant.StockQuantity += quantity;
            }

            _context.CartItems.Update(cartItem);
            _context.Entry(variant).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok("Updated cart successfully");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error updating cart item: {ex.Message}");
        }
    }


}