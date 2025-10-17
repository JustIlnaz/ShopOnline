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
    private List<Product> _allProducts;
    
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
        _allProducts = App.DbContext.Products
            .Include(p => p.Category)
            .ToList();

        UpdateProductsList();
    }

    private void UpdateProductsList(int? categoryId = null)
    {
             var query = _allProducts.AsQueryable();

        // Apply category filter
        if (categoryId.HasValue && categoryId.Value != 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Apply sorting
        if (SortComboBox.SelectedItem is ComboBoxItem selectedSort)
        {
            query = selectedSort.Content.ToString() switch
            {
                "Названию А-Я" => query.OrderBy(p => p.NameProducts),
                "Названию Я-А" => query.OrderByDescending(p => p.NameProducts),
                "Цене (возр.)" => query.OrderBy(p => decimal.Parse(p.Price ?? "0")),
                "Цене (убыв.)" => query.OrderByDescending(p => decimal.Parse(p.Price ?? "0")),
                _ => query
            };
        }

        ProductsDataGrid.ItemsSource = query.ToList();
    }

    private void OnCategoryChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is Category category)
        {
            UpdateProductsList(category.IdCategories);
        }
    }

    private void OnSortChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is Category category)
        {
            UpdateProductsList(category.IdCategories);
        }
    }

    private void AddToBasket(object? sender, RoutedEventArgs e)
    {
        var selectedProduct = ProductsDataGrid.SelectedItem as Product;
        if (selectedProduct == null)
        {
            // TODO: Show error message
            return;
        }

        try
        {
            var currentUser = ContextData.CurrentUser;
            if (currentUser == null) return;

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
                existingItem.Count = (int.Parse(existingItem.Count ?? "0") + 1).ToString();
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

            App.DbContext.SaveChanges();
            // TODO: Show success message
        }
        catch (Exception ex)
        {
            // TODO: Show error message
        }
    }
}