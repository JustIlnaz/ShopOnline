using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class OrderItem
{
    public int IdOrderItems { get; set; }

    public int ProductsId { get; set; }

    public int OrdersId { get; set; }

    public string? Count { get; set; }

    public string? PriceZaEd { get; set; }

    public virtual Order Orders { get; set; } = null!;

    public virtual Product Products { get; set; } = null!;
}
