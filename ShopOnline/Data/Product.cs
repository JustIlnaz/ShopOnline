using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class Product
{
    public int IdProducts { get; set; }

    public string? NameProducts { get; set; }

    public string? Description { get; set; }

    public string? Price { get; set; }

    public int? CountInSklade { get; set; }

    public DateTime? DateSave { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
