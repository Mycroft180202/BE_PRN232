using BE_PRN232.Configs;
using BE_PRN232.Entities;
using BE_PRN232.Model;
using BE_PRN232.RequestDTO;
using BE_PRN232.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BE_PRN232.Controllers;
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly EcommerceClothingDbContext _context;
    private readonly AppSettings _appSettings;
    public OrdersController(EcommerceClothingDbContext context,AppSettings appSettings)
    {
        _context = context;
        _appSettings = appSettings;
    }

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder( [FromBody] OrderRequest request)
    {
        try
        {
            var messages = new List<string>();
            var cart = await _context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Variant) 
                .FirstOrDefaultAsync(c => c.UserId.ToString() == request.UserId);

            if (cart == null)
                return NotFound("Cart not found for user.");

            var selectedCartItems = cart.CartItems
                .Where(c => request.cartItemIds.Contains(c.CartItemId))
                .ToList();

            if (!selectedCartItems.Any())
                return BadRequest("No matching cart items found.");
            var voucher = await _context.Vouchers.FindAsync(request.VoucherId);
            var subtotal = selectedCartItems.Sum(c => c.Quantity * c.Variant.Price);
            var totalAmount = subtotal - request.DiscountAmount;
            var curentDate = DateTime.Now;
            if (voucher != null)
            {
                if (curentDate > voucher.StartDate && curentDate < voucher.EndDate && 
                    subtotal >= voucher.MinOrderAmount && voucher.UsageLimit >0 && voucher.UsedCount < voucher.UsageLimit)
                {
                    totalAmount -= voucher.DiscountValue;
                    voucher.UsedCount++;
                    _context.Vouchers.Update(voucher);
                    messages.Add($"Voucher {voucher.Code} has been used.");
                }
            }
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = cart.UserId,
                OrderDate = DateTime.Now,
                DiscountAmount = request.DiscountAmount,
                VoucherId = request.VoucherId,
                Subtotal = subtotal,
                Notes = request.Notes,
                ShippingFee = request.ShippingFee,
                OrderStatus = OrderStatus.Pending.ToString(), 
                ShippingAddress = request.ShippingAddress,
                OrderItems = selectedCartItems.Select(ci => new OrderItem
                {
                    VariantId = ci.VariantId,
                    Quantity = ci.Quantity,
                    Price = ci.Variant.Price
                }).ToList(),
                TotalAmount = totalAmount,
                
            };

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(selectedCartItems);

            await _context.SaveChangesAsync();
            messages.Add($"Order has been created with orderId {order.OrderId}");
            return Ok(new { message = messages.ToList(), });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("order-history/{userId}")]
    public async Task<IActionResult> GetOrderHistory([FromRoute]string userId)
    {
        try
        {
            var baseUrl = _appSettings.BaseUrl;
            var orders = await _context.Orders
                .Include(o=>o.User)
                .Include(o=>o.Voucher)
                .Where(o=>o.UserId.ToString() == userId).ToListAsync();
            var response = orders.Select(o => new OrderResponse(o,baseUrl)).ToList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("order-history/order-detail/{orderId}")]
    public async Task<IActionResult> GetOrderHistoryDetail([FromRoute]string orderId)
    {
        try
        {
            var baseUrl = _appSettings.BaseUrl;
            var order = await _context.Orders
                .Include(o=>o.User)
                .Include(o=>o.Voucher)
                .FirstOrDefaultAsync(o=>o.OrderId.ToString() == orderId);
            var response = new OrderResponse(order,baseUrl);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}