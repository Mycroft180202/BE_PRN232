﻿using System;
using System.Collections.Generic;

namespace BE_PRN232.Entities;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public Guid OrderId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ProductVariant Variant { get; set; } = null!;
}
