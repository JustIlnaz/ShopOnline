using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class BasketItem
{
    public int IdBasketItems { get; set; }

    public int BasketId { get; set; }

    public int ProductsId { get; set; }

    public string? Count { get; set; }

    public virtual Basket Basket { get; set; } = null!;

    public virtual Product Products { get; set; } = null!;
}
