using System.ComponentModel.DataAnnotations;
namespace BE_PRN232.RequestDTO;

public class NewProductImage
{
    [Required]
    public int ProductId { get; set; }

    public string? AltText { get; set; }

    public bool IsThumbnail { get; set; } = false;
    [Required]
    public IFormFile ImageFile { get; set; } 
}