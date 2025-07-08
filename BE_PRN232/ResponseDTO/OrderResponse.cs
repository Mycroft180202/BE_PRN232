using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class OrderResponse
{
    public OrderResponse(Order order,string baseUrl)
    {
        Id = order.OrderId;
        UserId = order.UserId.ToString();
        OrderDate = order.OrderDate;
        OrderStatus = order.OrderStatus;
        ShippingAddress = order.ShippingAddress;
        ShippingFee = order.ShippingFee;
        Subtotal = order.Subtotal;
        DiscountAmount = order.DiscountAmount;
        VoucherCode = order.Voucher?.Code;
        TotalAmount = order.TotalAmount;
        Notes = order.Notes;
        OrderItems = order.OrderItems.Select(o=>new OrderItemResponse(o,baseUrl)).ToList();
    }
    public Guid Id { get; set; }

    public string UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; }

    public string ShippingAddress { get; set; } 

    public decimal ShippingFee { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public string? VoucherCode { get; set; } 
    public List<OrderItemResponse> OrderItems { get; set; } 
}