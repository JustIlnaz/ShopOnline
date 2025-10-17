using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using System.Linq;

namespace ShopOnline.Views.Manager;

public partial class ManagerPurchasesUserControl : UserControl
{
    private readonly AppDbContext _context;

    public ManagerPurchasesUserControl()
    {
        InitializeComponent();
        _context = new AppDbContext();
        LoadPurchases();
    }

    private void LoadPurchases()
    {
        var orders = _context.Orders
            .Include(o => o.Users)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Products)
            .Select(o => new {
                o.IdOrders,
                UserEmail = o.Users.Email,
                OrderDate = o.DateZakaza,
                TotalSum = o.SumAll,
                Status = o.Status
            })
            .ToList();
        PurchasesGrid.ItemsSource = orders;
    }

    private void OnSearch(object? sender, RoutedEventArgs e)
    {
        var searchText = SearchBox.Text?.ToLower() ?? "";
        var filteredOrders = _context.Orders
            .Include(o => o.Users)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Products)
            .Where(o => o.Users.Email.ToLower().Contains(searchText))
            .Select(o => new {
                o.IdOrders,
                UserEmail = o.Users.Email,
                OrderDate = o.DateZakaza,
                TotalSum = o.SumAll,
                Status = o.Status
            })
            .ToList();
        PurchasesGrid.ItemsSource = filteredOrders;
    }
}