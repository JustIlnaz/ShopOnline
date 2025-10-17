using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using System.Linq;
using System;
using Avalonia.VisualTree;

namespace ShopOnline.Views.Manager;

public partial class ManagerGoodsUserControl : UserControl
{
    public ManagerGoodsUserControl()
    {
        InitializeComponent();
        LoadProducts();
    }

    private void LoadProducts()
    {
        var products = App.DbContext.Products
            .Include(p => p.Category)
            .ToList();
        ProductsGrid.ItemsSource = products;
    }

    private void OnSearch(object? sender, RoutedEventArgs e)
    {
        var searchText = SearchBox.Text?.ToLower() ?? "";
        var filteredProducts = App.DbContext.Products
            .Include(p => p.Category)
            .Where(p => p.NameProducts.ToLower().Contains(searchText) ||
                       p.Description.ToLower().Contains(searchText))
            .ToList();
        ProductsGrid.ItemsSource = filteredProducts;
    }

    private async void OnAddProduct(object? sender, RoutedEventArgs e)
    {
        var window = new EditProductWindow();
        await window.ShowDialog(this.GetVisualRoot() as Window);
        LoadProducts();
    }

    private void OnDeleteProduct(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Product product)
        {
            App.DbContext.Products.Remove(product);
            App.DbContext.SaveChanges();
            LoadProducts();
        }
    }

    private async void ProductsGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (ProductsGrid.SelectedItem is Product selectedProduct)
        {
            var window = new EditProductWindow(selectedProduct);
            await window.ShowDialog(this.GetVisualRoot() as Window);
            LoadProducts();
        }
    }
}