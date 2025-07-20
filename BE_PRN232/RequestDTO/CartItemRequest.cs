using System.ComponentModel.DataAnnotations;
namespace BE_PRN232.RequestDTO;

public class CartItemRequest
{
    [Required]
    public int VariantId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}