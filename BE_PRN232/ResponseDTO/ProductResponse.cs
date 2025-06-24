using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class ProductResponse
{
    public ProductResponse(Product product,string baseUrl )
    {
        Id = product.ProductId;
        Name = product.Name;
        Description = product.Description;
        Category = new CategoryResponse(product.Category);
        Brand = new BrandResponse(product.Brand);
        Slug = product.Slug;
        IsPublished = product.IsPublished;
        CreatedAt = product.CreatedAt;
        UpdatedAt = product.UpdatedAt;
        Variants = product.ProductVariants.Select(v => new ProductVariantResponse(v)).ToList();
        Images = product.ProductImages.Select(i => new ProductImageResponse(i,baseUrl)).ToList();
    }
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public CategoryResponse Category { get; set; } = null!;
    
    public BrandResponse Brand { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    public List<ProductVariantResponse> Variants { get; set; } = new List<ProductVariantResponse>();

    public List<ProductImageResponse> Images { get; set; } = new List<ProductImageResponse>();

}