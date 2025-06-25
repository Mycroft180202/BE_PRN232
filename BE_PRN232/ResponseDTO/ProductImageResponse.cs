using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class ProductImageResponse
{
    public ProductImageResponse(ProductImage productImage,string baseUrl )
    {
        ImageUrl = $"{baseUrl}/images/{productImage.ImageUrl}";
        ProductId = productImage.ProductId;
        ImageId = productImage.ImageId;
        AltText = productImage.AltText;
        IsThumbnail = productImage.IsThumbnail;
    }
    public int ImageId { get; set; }

    public int ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public bool IsThumbnail { get; set; }
}