using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System.Linq;

namespace ShopOnline.Views.Customer;

public partial class CustomerOrdersUserControl : UserControl
{
    public CustomerOrdersUserControl()
    {
        InitializeComponent();
        LoadOrders();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        LoadOrders();
    }

    private void LoadOrders()
    {
        if (ContextData.CurrentUser == null) return;

        var orders = App.DbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Products)
            .Where(o => o.UsersId == ContextData.CurrentUser.IdUsers)
            .OrderByDescending(o => o.DateZakaza)
            .ToList();

        OrdersGrid.ItemsSource = orders;
    }
}