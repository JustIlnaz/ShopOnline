using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ShopOnline;

public partial class AdminPurchasesUserControl : UserControl
{
    public AdminPurchasesUserControl()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        LoadPurchases();
    }

    private void LoadPurchases()
    {
        var orders = App.DbContext.Orders
            .Include(o => o.Users)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Products)
            .OrderByDescending(o => o.DateZakaza)
            .SelectMany(o => o.OrderItems.Select(oi => new
            {
                OrderId = o.IdOrders,
                CustomerName = o.Users.FullName ?? "Unknown",
                ProductName = oi.Products.NameProducts,
                Count = oi.Count ?? "0",
                Price = oi.PriceZaEd ?? "0",
                OrderDate = o.DateZakaza
            }))
            .ToList();

        if (PurchasesDataGrid != null)
        {
            PurchasesDataGrid.ItemsSource = orders;
        }
    }
}