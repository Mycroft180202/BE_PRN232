using BE_PRN232.Entities;
namespace BE_PRN232.ResponseDTO;

public class CategoryResponse
{
    public CategoryResponse(Category category)
    {
        Id = category.CategoryId;
        Name = category.Name;
        Slug = category.Slug;
        Childrens = category.InverseParentCategory.Select(c => new CategoryChildren(c)).ToList();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public List<CategoryChildren>? Childrens { get; set; }
}
public class CategoryChildren 
{
    public CategoryChildren(Category category)
    {
        Id = category.CategoryId;
        Name = category.Name;
        Slug = category.Slug;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    
}
