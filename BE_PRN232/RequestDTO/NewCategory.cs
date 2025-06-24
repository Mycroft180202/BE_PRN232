using System.ComponentModel.DataAnnotations;
namespace BE_PRN232.RequestDTO;

public class NewCategory
{
    [Required]
    public string Name { get; set; } = String.Empty;
    [Required]
    public string Slug { get; set; } = String.Empty;
    
    public int? ParentCategoryId { get; set; }
 
    public bool AutoSEO  { get; set; } = false;
}