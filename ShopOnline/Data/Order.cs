using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class Order
{
    public int IdOrders { get; set; }

    public int UsersId { get; set; }

    public DateTime? DateZakaza { get; set; }

    public string? SumAll { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User Users { get; set; } = null!;
}
