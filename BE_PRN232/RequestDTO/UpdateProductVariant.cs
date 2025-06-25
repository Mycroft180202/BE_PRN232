using System.ComponentModel.DataAnnotations;
namespace BE_PRN232.RequestDTO;

public class UpdateProductVariant
{
    [Required]
    public int Id { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    public string? Sku { get; set; }
    private string size;
    [Required] 
    public string Size { get => size; set => size = value.ToUpper(); }
    private string color;
    [Required]
    public string Color { get=>color; set =>color = value.ToUpper(); } 
    [Required]
    public decimal Price { get; set; }
    
    public decimal? SalePrice { get; set; }
    [Required]
    public int StockQuantity { get; set; }
}