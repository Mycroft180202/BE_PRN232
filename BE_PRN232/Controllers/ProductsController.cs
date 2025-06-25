using BE_PRN232.Configs;
using BE_PRN232.Entities;
using BE_PRN232.Helpers;
using BE_PRN232.RequestDTO;
using BE_PRN232.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BE_PRN232.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly EcommerceClothingDbContext _context;
    private readonly AppSettings _appSettings;
    public ProductsController(EcommerceClothingDbContext context,AppSettings appSettings)
    {
        _context = context;
        _appSettings = appSettings;
    }
    /// <summary>
    /// get products
    /// </summary>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="category"></param>
    /// <param name="brand"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] int category = 0, [FromQuery] int brand = 0)
    {
        try
        {
            var query = _context.Products
                .Include(c => c.Category)
                .Include(c => c.Brand)
                .Include(p=>p.ProductVariants)
                .Include(p=>p.ProductImages)
                .AsQueryable();

            if (category > 0)
            {
                query = query.Where(x => x.CategoryId == category);
            }

            if (brand > 0)
            {
                query = query.Where(x => x.BrandId == brand);
            }
            var totalItems = await query.CountAsync(); 
            var totalPages = (int)Math.Ceiling(totalItems / (double)size); 
            var products = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
            var response = products.Select(p => new ProductResponse(p,_appSettings.BaseUrl));
            return Ok(new
            {
                currentPage = page,
                pageSize = size,
                totalItems,
                totalPages,
                data = response
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// create new product
    /// </summary>
    /// <param name="newProduct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NewProduct newProduct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var categoryExit = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(p=>p.CategoryId == newProduct.CategoryId);
            var brandExit = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(p=>p.BrandId == newProduct.BrandId);
            var slugExit = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p=>p.Slug == newProduct.Slug);
            var nameExit = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p=>p.Name == newProduct.Name);
            if(categoryExit== null) return BadRequest("Category not found");
            if(brandExit== null) return BadRequest("Brand not found");
            if(slugExit!= null) return BadRequest("Slug already exists");
            if(nameExit!=null) return BadRequest("Product name already exists");
            var product = new Product()
            {
                Name = newProduct.Name,
                Description = newProduct.Description,
                CategoryId = newProduct.CategoryId,
                BrandId = newProduct.BrandId,
                IsPublished = newProduct.IsPublished,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Slug = newProduct.Slug,
            };
            if (newProduct.AutoSeo)
            {
                product.Slug = SlugHelper.GenerateSlug(newProduct.Name);
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok("Created product successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// update product
    /// </summary>
    /// <param name="updateProduct"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProduct updateProduct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var productExit = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == updateProduct.Id);
            var exitCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(p=>p.CategoryId == updateProduct.CategoryId);
            var exitBrand  = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(p => p.BrandId == updateProduct.BrandId);
            if (exitCategory == null) return BadRequest("Category not found");
            if (exitBrand == null) return BadRequest("Brand not found");
            if(productExit != null) return NotFound("Can not found product");
            if (updateProduct.Name != productExit.Name && _context.Products.AsNoTracking().Any(p => p.Name == productExit.Name))
            {
                return BadRequest("New name already exists");
            }
            productExit.Name = updateProduct.Name;
            productExit.Description = updateProduct.Description;
            productExit.CategoryId = updateProduct.CategoryId;
            productExit.BrandId = updateProduct.BrandId;
            productExit.IsPublished = updateProduct.IsPublished;
            productExit.UpdatedAt = DateTime.Now;
            productExit.Slug = updateProduct.Slug;
            if (updateProduct.AutoSeo)
            {
                productExit.Slug = SlugHelper.GenerateSlug(updateProduct.Name);
            }
            _context.Products.Update(productExit);
            await _context.SaveChangesAsync();
            return Ok("Updated product successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// get by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            var product = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.Brand)
                .Include(p=>p.ProductVariants)
                .Include(p=>p.ProductImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if(product == null) return NotFound("Product not found");
            var response = new ProductResponse(product,_appSettings.BaseUrl);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    /// <summary>
    /// get product by slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    [HttpGet("/slug/{slug}")]
    public async Task<IActionResult> GetBySlug([FromRoute] string slug)
    {
        try
        {
            var product = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.Brand)
                .Include(p=>p.ProductVariants)
                .Include(p=>p.ProductImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug);
            if(product == null) return NotFound("Product not found");
            var response = new ProductResponse(product,_appSettings.BaseUrl);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// create new variant 
    /// </summary>
    /// <param name="newProductVariant"></param>
    /// <returns></returns>
    [HttpPost("/create/variant")]
    public async Task<IActionResult> CreateVariantProduct([FromBody] NewProductVariant newProductVariant)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var variants = await _context.ProductVariants.ToListAsync();
            var productExit = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == newProductVariant.ProductId);
            var colorSizeExit = variants.Any(p=>p.Color.ToUpper() == newProductVariant.Color
                                            && p.Size.ToUpper() == newProductVariant.Size);
            var skuExit = variants.Any(v=>v.Sku == newProductVariant.Sku);
            if(productExit == null) return BadRequest("ProductId not found");
            if(colorSizeExit) return BadRequest("Color size already exists");
            if(skuExit) return BadRequest("Sku already exists");
            var variant = new ProductVariant()
            {
                ProductId = newProductVariant.ProductId,
                Sku = newProductVariant.Sku,
                Size = newProductVariant.Size,
                Color = newProductVariant.Color,
                Price = newProductVariant.Price,
                SalePrice = newProductVariant.SalePrice,
                StockQuantity = newProductVariant.StockQuantity,
            };
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();
            return Ok("Created variant successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// update variant
    /// </summary>
    /// <param name="updateProductVariant"></param>
    /// <returns></returns>
    [HttpPut("/update/variant")]
    public async Task<IActionResult> UpdateVariantProduct([FromBody] UpdateProductVariant updateProductVariant)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var variants = await _context.ProductVariants.ToListAsync();
            var variant = variants.FirstOrDefault(p => p.ProductId == updateProductVariant.ProductId);
            if(variant == null) return NotFound("Variant not found");
            var productExit = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == updateProductVariant.ProductId);
            if(productExit == null) return BadRequest("ProductId not found");
            var colorSizeExists =  variants.Any(p => p.ProductId == updateProductVariant.ProductId &&
                                                     p.VariantId != updateProductVariant.Id &&
                                                     p.Color.ToUpper() == updateProductVariant.Color.ToUpper() &&
                                                     p.Size.ToUpper() == updateProductVariant.Size.ToUpper());
            if (colorSizeExists)
                return BadRequest("Color and Size combination already exists for another variant.");
            variant.Price = updateProductVariant.Price;
            variant.SalePrice = updateProductVariant.SalePrice;
            variant.StockQuantity = updateProductVariant.StockQuantity;
            variant.Sku = updateProductVariant.Sku;
            variant.Color = updateProductVariant.Color;
            variant.Size = updateProductVariant.Size;
            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();
            return Ok("Updated variant successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    /// <summary>
    /// create image product
    /// </summary>
    /// <param name="newProductImage"></param>
    /// <returns></returns>
    [HttpPost("create/image")]
    public async Task<IActionResult> CreateImageProduct([FromForm] NewProductImage newProductImage)
    {
        try
        {
            var file = newProductImage.ImageFile;
            if (file == null ||file.Length == 0)
                return BadRequest("No file provided.");
            // Lưu file mới
            var fileName = SaveFile(file);
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Failed to save file.");
            var image = new ProductImage()
            {
                ProductId = newProductImage.ProductId,
                ImageUrl = fileName,
                IsThumbnail = newProductImage.IsThumbnail,
                AltText = newProductImage.AltText,
            };
            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
            return Ok("Created image successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
    private string SaveFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var folderPath = _appSettings.PathImage;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        var filePath = Path.Combine(folderPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }
        return fileName;
    }
    /// <summary>
    /// update image
    /// </summary>
    /// <param name="updateImage"></param>
    /// <returns></returns>
    [HttpPost("update/image")]
    public async Task<IActionResult> UpdateImage([FromForm] UpdateProductImage updateImage)
    {
        try
        {
            var existingImage = await _context.ProductImages.FindAsync(updateImage.Id);
            if (existingImage == null)
                return NotFound("Image not found.");

            if (updateImage.ImageFile != null && updateImage.ImageFile.Length > 0)
            {
                var oldPath = Path.Combine(_appSettings.PathImage, existingImage.ImageUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                var newFileName = SaveFile(updateImage.ImageFile);
                if (string.IsNullOrEmpty(newFileName))
                    return BadRequest("Failed to save new file.");
                existingImage.ImageUrl = newFileName;
            }
            existingImage.IsThumbnail = updateImage.IsThumbnail;
            existingImage.AltText = updateImage.AltText;
            _context.ProductImages.Update(existingImage);
            await _context.SaveChangesAsync();
            return Ok("Image updated successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    /// <summary>
    /// delete image
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/image/{id}")]
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image == null)
                return NotFound("Image not found.");

            var imagePath = Path.Combine(_appSettings.PathImage, image.ImageUrl);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return Ok("Image deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}