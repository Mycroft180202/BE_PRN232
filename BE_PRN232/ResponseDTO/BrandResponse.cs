using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class BrandResponse
{
    public BrandResponse(Brand brand)
    {
        Id = brand.BrandId;
        Name = brand.Name;
    }
    public int Id { get; set; }
    public string Name { get; set; }
}