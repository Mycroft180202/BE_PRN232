using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class ProductVariantResponse
{
    public ProductVariantResponse(ProductVariant productVariant)
    {
        ProductId = productVariant.ProductId;
        VariantId = productVariant.VariantId;
        Sku = productVariant.Sku;
        Price = productVariant.Price;
        Size = productVariant.Size;
        Color = productVariant.Color;
        SalePrice = productVariant.SalePrice;
        StockQuantity = productVariant.StockQuantity;
    }
    
    public int VariantId { get; set; }

    public int ProductId { get; set; }

    public string? Sku { get; set; }

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal? SalePrice { get; set; }

    public int StockQuantity { get; set; }
}