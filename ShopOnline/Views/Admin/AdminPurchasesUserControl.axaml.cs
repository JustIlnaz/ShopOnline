using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Diagnostics;
using System.Collections.Generic;

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
        try
        {
            // First, get the data from the database
            var ordersQuery = App.DbContext.Orders
                .AsNoTracking()
                .Include(o => o.Users)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Products)
                .OrderByDescending(o => o.DateZakaza)
                .ToList(); // Execute the query here

            // Then process the results in memory
            var orders = ordersQuery
                .SelectMany(o => o.OrderItems.Select(oi => new OrderItemDisplay
                {
                    OrderId = o.IdOrders,
                    CustomerName = o.Users != null ? (string.IsNullOrEmpty(o.Users.FullName) ? "Unknown" : o.Users.FullName) : "Unknown",
                    ProductName = oi.Products != null ? (string.IsNullOrEmpty(oi.Products.NameProducts) ? "Unknown Product" : oi.Products.NameProducts) : "Unknown Product",
                    Count = string.IsNullOrEmpty(oi.Count) ? "0" : oi.Count,
                    Price = string.IsNullOrEmpty(oi.PriceZaEd) ? "0" : oi.PriceZaEd,
                    OrderDate = o.DateZakaza
                }))
                .ToList();

            if (PurchasesDataGrid != null)
            {
                PurchasesDataGrid.ItemsSource = orders;
                Debug.WriteLine($"Loaded {orders.Count} order items");
            }
            else
            {
                Debug.WriteLine("PurchasesDataGrid is null!");
            }
        }
        catch (ObjectDisposedException)
        {
            Debug.WriteLine("DbContext was disposed, attempting to refresh the view...");
            // You might want to add a retry mechanism here or show a message to the user
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading purchases: {ex}");
            // You might want to show an error message to the user here
        }
    }
}

public class OrderItemDisplay
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string Count { get; set; } = "0";
    public string Price { get; set; } = "0";
    public DateTime? OrderDate { get; set; }
}