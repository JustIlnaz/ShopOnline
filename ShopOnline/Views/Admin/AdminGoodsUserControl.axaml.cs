using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System.Linq;
using System;
using System.Diagnostics;

namespace ShopOnline;

public partial class AdminGoodsUserControl : UserControl
{
    public AdminGoodsUserControl()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        LoadProducts();
    }

    private void LoadProducts()
    {
       
            var products = App.DbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .ToList();

            Debug.WriteLine($"Loaded {products.Count} products");
            
            foreach (var product in products)
            {
                Debug.WriteLine($"Product: {product.NameProducts}, Category: {product.Category?.NameCategories ?? "No Category"}");
            }

            if (ProductsDataGrid != null)
            {
                ProductsDataGrid.ItemsSource = null; // Clear existing items
                ProductsDataGrid.ItemsSource = products; // Set new items
                Debug.WriteLine("DataGrid ItemsSource set");
            }
            else
            {
                Debug.WriteLine("ProductsDataGrid is null!");
            }
        
        
    }

    private void AddProduct(object? sender, RoutedEventArgs e)
    {
        ContextData.selectedProductInMainWindow = null;
        var window = new GoodsManagementWindow();
        window.Closed += (s, e) => LoadProducts();
        window.Show();
    }

    private void EditProduct(object? sender, RoutedEventArgs e)
    {
        var selectedProduct = ProductsDataGrid.SelectedItem as Product;
        if (selectedProduct == null) return;

        ContextData.selectedProductInMainWindow = selectedProduct;
        var window = new GoodsManagementWindow();
        window.Closed += (s, e) => LoadProducts();
        window.Show();
    }

    private void DeleteProduct(object? sender, RoutedEventArgs e)
    {
        var selectedProduct = ProductsDataGrid.SelectedItem as Product;
        if (selectedProduct == null) return;

        var product = App.DbContext.Products.FirstOrDefault(x => x.IdProducts == selectedProduct.IdProducts);
        if (product == null) return;

        App.DbContext.Products.Remove(product);
        App.DbContext.SaveChanges();
        LoadProducts();
    }

    private void ProductsDataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        EditProduct(sender, e);
    }
}