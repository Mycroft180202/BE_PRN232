namespace BE_PRN232.RequestDTO;

public class UpdateProductImage
{
    public int Id { get; set; }
    public IFormFile? ImageFile { get; set; }
    public bool IsThumbnail { get; set; }
    public string? AltText { get; set; }
}