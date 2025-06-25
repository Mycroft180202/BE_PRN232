using System;
using System.Collections.Generic;

namespace BE_PRN232.Entities;

public partial class ProductVariant
{
    public int VariantId { get; set; }

    public int ProductId { get; set; }

    public string? Sku { get; set; }

    public string Size { get; set; } = null!;

    public string Color { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? SalePrice { get; set; }

    public int StockQuantity { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;
}
