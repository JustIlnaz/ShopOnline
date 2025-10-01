using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class User
{
    public int IdUsers { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Description { get; set; }

    public int RoleId { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;
}
