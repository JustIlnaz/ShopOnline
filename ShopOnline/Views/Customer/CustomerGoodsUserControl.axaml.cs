using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopOnline.Views.Customer;

public partial class CustomerGoodsUserControl : UserControl
{
    public CustomerGoodsUserControl()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        // Load categories
        var categories = App.DbContext.Categories.ToList();
        categories.Insert(0, new Category { IdCategories = 0, NameCategories = "Все категории" });
        CategoryComboBox.ItemsSource = categories;
        CategoryComboBox.DisplayMemberBinding = new Avalonia.Data.Binding("NameCategories");
        CategoryComboBox.SelectedIndex = 0;

        // Load products
        App.DbContext.Products
            .Include(p => p.Category)
            .Load();

        UpdateProductsList();
    }

    private void UpdateProductsList(int? categoryId = null, string? searchText = null)
    {
        var query = App.DbContext.Products.AsQueryable();

        // Apply category filter
        if (categoryId.HasValue && categoryId.Value != 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(p => p.NameProducts != null &&
                                    p.NameProducts.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        // Load data first
        var products = query.ToList();

        // Apply sorting in memory
        if (SortComboBox.SelectedItem is ComboBoxItem selectedSort)
        {
            products = selectedSort.Content.ToString() switch
            {
                "Название А-Я" => products.OrderBy(p => p.NameProducts).ToList(),
                "Название Я-А" => products.OrderByDescending(p => p.NameProducts).ToList(),
                "Цена (по возрастанию)" => products.OrderBy(p => decimal.Parse(p.Price ?? "0")).ToList(),
                "Цена (по убыванию)" => products.OrderByDescending(p => decimal.Parse(p.Price ?? "0")).ToList(),
                _ => products
            };
        }

        ProductsDataGrid.ItemsSource = products;
    }

    private void OnCategoryChanged(object? sender, SelectionChangedEventArgs e)
    {
        var category = CategoryComboBox.SelectedItem as Category;
        var searchText = SearchTextBox?.Text;
        UpdateProductsList(category?.IdCategories, searchText);
    }

    private void OnSortChanged(object? sender, SelectionChangedEventArgs e)
    {
        var category = CategoryComboBox.SelectedItem as Category;
        var searchText = SearchTextBox?.Text;
        UpdateProductsList(category?.IdCategories, searchText);
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        var category = CategoryComboBox.SelectedItem as Category;
        var searchText = SearchTextBox?.Text;
        UpdateProductsList(category?.IdCategories, searchText);
    }

    private void ClearFilters(object? sender, RoutedEventArgs e)
    {
        CategoryComboBox.SelectedIndex = 0;
        SearchTextBox.Text = "";
        SortComboBox.SelectedIndex = 0;
        UpdateProductsList();
    }

    private void AddToBasket(object? sender, RoutedEventArgs e)
    {
        var selectedProduct = ProductsDataGrid.SelectedItem as Product;
        if (selectedProduct == null)
        {
            ShowMessage("Пожалуйста, выберите товар для добавления в корзину", "Внимание");
            return;
        }

        if (selectedProduct.CountInSklade <= 0)
        {
            ShowMessage("Товар отсутствует на складе", "Ошибка");
            return;
        }

        try
        {
            var currentUser = ContextData.CurrentUser;
            if (currentUser == null)
            {
                ShowMessage("Пользователь не авторизован", "Ошибка");
                return;
            }

            // Get or create current basket
            var basket = App.DbContext.Baskets
                .Include(b => b.BasketItems)
                .FirstOrDefault(b => b.UsersId == currentUser.IdUsers)
                ?? new Basket
                {
                    UsersId = currentUser.IdUsers,
                    DateStart = DateTime.Now
                };

            if (basket.IdBasket == 0) // New basket
            {
                App.DbContext.Baskets.Add(basket);
                App.DbContext.SaveChanges();
            }

            // Add or update basket item
            var existingItem = basket.BasketItems
                .FirstOrDefault(bi => bi.ProductsId == selectedProduct.IdProducts);

            if (existingItem != null)
            {
                var currentCount = int.Parse(existingItem.Count ?? "0");
                if (currentCount >= selectedProduct.CountInSklade)
                {
                    ShowMessage("Недостаточно товара на складе", "Ошибка");
                    return;
                }
                existingItem.Count = (currentCount + 1).ToString();
            }
            else
            {
                var newItem = new BasketItem
                {
                    BasketId = basket.IdBasket,
                    ProductsId = selectedProduct.IdProducts,
                    Count = "1"
                };
                App.DbContext.BasketItems.Add(newItem);
            }

            // Update product stock
            selectedProduct.CountInSklade = Math.Max(0, (selectedProduct.CountInSklade ?? 0) - 1);

            App.DbContext.SaveChanges();
            ShowMessage($"Товар '{selectedProduct.NameProducts}' добавлен в корзину", "Успех");
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при добавлении товара: {ex.Message}", "Ошибка");
        }
    }

    private void ShowMessage(string message, string title)
    {
        var messageBox = new Window
        {
            Title = title,
            Content = new TextBlock { Text = message, Margin = new Avalonia.Thickness(10) },
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        messageBox.Show();
    }
}