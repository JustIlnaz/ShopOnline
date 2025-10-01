using System;
using System.Collections.Generic;

namespace ShopOnline.Data;

public partial class Basket
{
    public int IdBasket { get; set; }

    public int UsersId { get; set; }

    public DateTime? DateStart { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    public virtual User Users { get; set; } = null!;
}
