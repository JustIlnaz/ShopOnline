using ShopOnline.Data;

namespace ShopOnline.Views.Customer;

public class BasketItemViewModel
{
    public BasketItem BasketItem { get; set; }
    
    public string CalculatedTotalPrice
    {
        get
        {
            var price = decimal.Parse(BasketItem.Products.Price ?? "0");
            var count = decimal.Parse(BasketItem.Count ?? "0");
            return (price * count).ToString("F2");
        }
    }
    
    public BasketItemViewModel(BasketItem basketItem)
    {
        BasketItem = basketItem;
    }
}
