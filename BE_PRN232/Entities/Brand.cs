using System;
using System.Collections.Generic;

namespace BE_PRN232.Entities;

public partial class Brand
{
    public int BrandId { get; set; }

    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
