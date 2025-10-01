using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class Role
{
    public int IdRoles { get; set; }

    public string? NameRole { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
