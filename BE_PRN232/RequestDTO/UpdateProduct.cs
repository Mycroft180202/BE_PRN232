using System.ComponentModel.DataAnnotations;
namespace BE_PRN232.RequestDTO;

public class UpdateProduct
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = String.Empty;

    public string? Description { get; set; } = String.Empty;
    [Required]
    public int CategoryId { get; set; }
    [Required]
    public int BrandId { get; set; }

    public string Slug { get; set; } = string.Empty;

    public bool IsPublished { get; set; } = true;
    
    public bool AutoSeo { get; set; } = true;
}