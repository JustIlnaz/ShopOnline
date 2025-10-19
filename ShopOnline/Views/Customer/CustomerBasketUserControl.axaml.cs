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
            App.DbContext.Baskets.ToList();
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
                    var viewModels = _currentBasket.BasketItems.Select(item => new BasketItemViewModel(item)).ToList();
                    BasketItemsGrid.ItemsSource = viewModels;
                    UpdateTotal();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateTotal()
        {
            if (_currentBasket == null || !_currentBasket.BasketItems.Any())
            {
                TotalText.Text = "Итого: 0 руб.";
                ItemCountText.Text = "Корзина пуста";
                return;
            }

            var total = _currentBasket.BasketItems.Sum(bi =>
                decimal.Parse(bi.Products.Price ?? "0") * decimal.Parse(bi.Count ?? "0"));
            var itemCount = _currentBasket.BasketItems.Sum(bi => int.Parse(bi.Count ?? "0"));

            TotalText.Text = $"Итого: {total:F2} руб.";
            ItemCountText.Text = $"Товаров в корзине: {itemCount}";
        }

        private void IncreaseCount(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BasketItemViewModel viewModel)
            {
                var item = viewModel.BasketItem;
                var count = int.Parse(item.Count ?? "0");
                var availableStock = item.Products.CountInSklade ?? 0;
                
                if (count >= availableStock)
                {
                    ShowMessage("Недостаточно товара на складе", "Внимание");
                    return;
                }
                
                item.Count = (count + 1).ToString();
                // Update product stock
                item.Products.CountInSklade = Math.Max(0, availableStock - 1);
                
                App.DbContext.SaveChanges();
                UpdateTotal();
                LoadBasket();
            }
        }

        private void DecreaseCount(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BasketItemViewModel viewModel)
            {
                var item = viewModel.BasketItem;
                var count = int.Parse(item.Count ?? "0");
                if (count > 1)
                {
                    item.Count = (count - 1).ToString();
                    // Return one item to stock
                    item.Products.CountInSklade = (item.Products.CountInSklade ?? 0) + 1;
                    
                    App.DbContext.SaveChanges();
                    UpdateTotal();
                    LoadBasket();
                }
            }
        }

        private void RemoveItem(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BasketItemViewModel viewModel)
            {
                var item = viewModel.BasketItem;
                var count = int.Parse(item.Count ?? "0");
                
                // Return all items to stock
                item.Products.CountInSklade = (item.Products.CountInSklade ?? 0) + count;
                
                App.DbContext.BasketItems.Remove(item);
                App.DbContext.SaveChanges();
                LoadBasket();
            }
        }

        private void ClearBasket(object? sender, RoutedEventArgs e)
        {
            if (_currentBasket == null || !_currentBasket.BasketItems.Any())
            {
                ShowMessage("Корзина уже пуста", "Информация");
                return;
            }

            try
            {
                // Return all items to stock before clearing basket
                foreach (var item in _currentBasket.BasketItems)
                {
                    var count = int.Parse(item.Count ?? "0");
                    item.Products.CountInSklade = (item.Products.CountInSklade ?? 0) + count;
                }
                
                App.DbContext.BasketItems.RemoveRange(_currentBasket.BasketItems);
                App.DbContext.SaveChanges();
                LoadBasket();
                ShowMessage("Корзина очищена", "Информация");
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при очистке корзины: {ex.Message}", "Ошибка");
            }
        }

        private void CreateOrder(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentBasket == null || !_currentBasket.BasketItems.Any())
                {
                    return;
                }

                // Create order
                var order = new Order
                {
                    UsersId = _currentBasket.UsersId,
                    DateZakaza = DateTime.Now,
                    Status = "В пути",
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
            }
        catch (Exception ex)
        {
        }
    }

    private void ShowMessage(string message, string title)
    {
        var messageBox = new Window
        {
            Title = title,
            Content = new TextBlock { Text = message, Margin = new Avalonia.Thickness(10) },
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        messageBox.Show();
    }
}