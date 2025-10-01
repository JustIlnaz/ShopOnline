using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class Category
{
    public int IdCategories { get; set; }

    public string? NameCategories { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
