using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class OrderItemResponse
{
 
    public OrderItemResponse(OrderItem orderItem,string baseUrl)
    {
        Id= orderItem.OrderItemId;
        OrderId = orderItem.OrderId.ToString();
        var product = orderItem.Variant.Product;
        ProductVariant = new ProductVariantResponse(orderItem.Variant);
        Product = new ProductResponse(product, baseUrl);
        Quantity = orderItem.Quantity;
        Price = orderItem.Price;
    }
    
    public int Id { get; set; }

    public string OrderId { get; set; }

    public ProductVariantResponse ProductVariant { get; set; }
    public ProductResponse Product { get; set; }
    public int Quantity { get; set; }

    public decimal Price { get; set; }
}