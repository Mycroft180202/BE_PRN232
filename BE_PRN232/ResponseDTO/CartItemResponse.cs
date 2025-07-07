using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class CartItemResponse
{
    public CartItemResponse()
    {
       
    }
    public CartItemResponse(CartItem cartItem,string baseUrl)
    {
        Id = cartItem.CartItemId;
        VariantId = cartItem.VariantId;
        Quantity = cartItem.Quantity;
        var variant = cartItem.Variant;
        ProductId = variant.ProductId;
        ProductName = variant.Product.Name;
        Sku = variant.Sku;
        Size = variant.Size;
        Color = variant.Color;
        Price = variant.Price;
        SalePrice = variant.SalePrice;
        StockQuantity = variant.StockQuantity;
        ImageUrl = $"{baseUrl}/images/{variant.Product.ProductImages.FirstOrDefault().ImageUrl}";
    }
    public int Id { get; set; }
    
    public int VariantId { get; set; }

    public int Quantity { get; set; }
    
    public int ProductId { get; set; }
    
    public string ImageUrl { get; set; }
    public string? ProductName { get; set; } = string.Empty;

    public string? Sku { get; set; }

    public string? Size { get; set; } 

    public string? Color { get; set; } 

    public decimal ?Price { get; set; }

    public decimal? SalePrice { get; set; }

    public int? StockQuantity { get; set; }
    
}