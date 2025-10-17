using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Linq;

namespace ShopOnline.Views.Customer;

public partial class CustomerBasketUserControl : UserControl
{
    private Basket? _currentBasket;

    public CustomerBasketUserControl()
    {
        InitializeComponent();
        LoadBasket();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        LoadBasket();
    }

    private void LoadBasket()
    {
        try
        {
            if (ContextData.CurrentUser == null) return;

            _currentBasket = App.DbContext.Baskets
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Products)
                .FirstOrDefault(b => b.UsersId == ContextData.CurrentUser.IdUsers);

            if (_currentBasket != null)
            {
                BasketItemsGrid.ItemsSource = _currentBasket.BasketItems.ToList();
                UpdateTotal();
            }
        }
        catch (Exception ex)
        {
            // TODO: Show error
        }
    }

    private void UpdateTotal()
    {
        if (_currentBasket == null)
        {
            TotalText.Text = "Итого: 0 руб.";
            return;
        }

        var total = _currentBasket.BasketItems.Sum(bi =>
            decimal.Parse(bi.Products.Price ?? "0") * decimal.Parse(bi.Count ?? "0"));
        TotalText.Text = $"Итого: {total} руб.";
    }

    private void IncreaseCount(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BasketItem item)
        {
            var count = int.Parse(item.Count ?? "0");
            item.Count = (count + 1).ToString();
            App.DbContext.SaveChanges();
            UpdateTotal();
        }
    }

    private void DecreaseCount(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BasketItem item)
        {
            var count = int.Parse(item.Count ?? "0");
            if (count > 1)
            {
                item.Count = (count - 1).ToString();
                App.DbContext.SaveChanges();
                UpdateTotal();
            }
        }
    }

    private void RemoveItem(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BasketItem item)
        {
            App.DbContext.BasketItems.Remove(item);
            App.DbContext.SaveChanges();
            LoadBasket();
        }
    }

    private void CreateOrder(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_currentBasket == null || !_currentBasket.BasketItems.Any())
            {
                // TODO: Show error about empty basket
                return;
            }

            // Create order
            var order = new Order
            {
                UsersId = _currentBasket.UsersId,
                DateZakaza = DateTime.Now,
                Status = "Новый",
                SumAll = _currentBasket.BasketItems.Sum(bi =>
                    decimal.Parse(bi.Products.Price ?? "0") * decimal.Parse(bi.Count ?? "0")).ToString()
            };

            App.DbContext.Orders.Add(order);
            App.DbContext.SaveChanges();

            // Create order items
            foreach (var basketItem in _currentBasket.BasketItems)
            {
                var orderItem = new OrderItem
                {
                    OrdersId = order.IdOrders,
                    ProductsId = basketItem.ProductsId,
                    Count = basketItem.Count,
                    PriceZaEd = basketItem.Products.Price
                };
                App.DbContext.OrderItems.Add(orderItem);
            }

            // Clear basket
            App.DbContext.BasketItems.RemoveRange(_currentBasket.BasketItems);
            App.DbContext.SaveChanges();

            LoadBasket();
            // TODO: Show success message
        }
        catch (Exception ex)
        {
            // TODO: Show error
        }
    }
}