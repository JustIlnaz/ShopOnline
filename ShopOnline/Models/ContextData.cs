using ShopOnline.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Models
{
    internal class ContextData
    {
        static public Login selectedLogin1InMainWindow { get; set; } 

        static public Role selectedRoleInMainWindow { get; set; }

        static public BasketItem selectedBasketItemInMainWindow { get;set; }

        static public Basket selectedBasketInMainWindow {  get; set; }

       static public Product selectedProductInMainWindow { get; set; }
        
        static public Category selectedCategoryInMainWindow { get; set; }   

        static public Order selectedOrderInMainWindow { get; set; }

        static public OrderItem selectedOrderItemInMainWindow { get; set; }
    }
}
