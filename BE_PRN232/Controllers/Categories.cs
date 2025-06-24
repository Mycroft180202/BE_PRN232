
using BE_PRN232.Entities;
using ManagementApi.Helpers;
using ManagementApi.RequestDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ManagementApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class Categories : ControllerBase
{
    private readonly EcommerceClothingDbContext _context;
    public Categories(EcommerceClothingDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// get all category
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        try
        {
           var cateParents = await _context.Categories
               .Include(c=>c.InverseParentCategory)
               .Where(c=>c.ParentCategoryId == null)
               .AsNoTracking()
               .ToListAsync();
           var response = cateParents.Select(c => new
           {
               Id = c.CategoryId,
               Name = c.Name,
               Slug = c.Slug,
               Children = c.InverseParentCategory.Select(cl => new
               {
                   Id = cl.CategoryId, Name = cl.Name, Slug = cl.Slug
               })
           });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// create new category
    /// </summary>
    /// <param name="newCategory"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(NewCategory newCategory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var parentCateIds = await _context.Categories
                .Where(c=>c.ParentCategoryId == null)
                .AsNoTracking()
                .Select(c => c.CategoryId)
                .ToListAsync();
            if (newCategory.ParentCategoryId != null && parentCateIds.Contains(newCategory.ParentCategoryId.Value))
            {
                var category = new Category()
                {
                    Name = newCategory.Name, Slug = newCategory.Slug, ParentCategoryId = newCategory.ParentCategoryId,

                };
                if (newCategory.AutoSEO)
                {
                    category.Slug = SlugHelper.GenerateSlug(category.Name);
                }
                if (_context.Categories.Any(c => c.Slug == category.Slug))
                {
                    return BadRequest("Category Slug already exists");
                }
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Ok("Category Created Successfully");
            }
            else
            {
                return BadRequest("Parent Category Id is invalid");
            }
           
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// delete category = update status 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Category>> DeleteCategory(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.Status = false;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return Ok("Category Deleted Successfully");
            }
            return NotFound("Category Not Found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// update category
    /// </summary>
    /// <param name="updateCategory"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<Category>> UpdateCategory(UpdateCategory updateCategory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            
            var category = await _context.Categories.FindAsync(updateCategory.Id);
            if (category != null)
            {
                var parentIds = await _context.Categories
                    .Where(c => c.ParentCategoryId == null)
                    .AsNoTracking()
                    .Select(c => c.CategoryId)
                    .ToListAsync();
                if (updateCategory.ParentCategoryId != null && !parentIds.Contains(updateCategory.ParentCategoryId.Value))
                {
                   return BadRequest("Parent Category Id is invalid");
                }
                category.Name = updateCategory.Name;
                category.Slug = updateCategory.Slug;
                category.ParentCategoryId = updateCategory.ParentCategoryId;
                if (updateCategory.AutoSEO)
                {
                    category.Slug = SlugHelper.GenerateSlug(category.Name);
                       
                }
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return Ok("Category Updated");
            }
            return NotFound("Category Not Found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}