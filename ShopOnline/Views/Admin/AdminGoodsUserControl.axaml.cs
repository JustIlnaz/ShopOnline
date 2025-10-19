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
        try
        {
            var products = App.DbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .ToList();

            Debug.WriteLine($"Loaded {products.Count} products");
            
            if (ProductsDataGrid != null)
            {
                ProductsDataGrid.ItemsSource = products;
                Debug.WriteLine("DataGrid ItemsSource set");
            }
            else
            {
                Debug.WriteLine("ProductsDataGrid is null!");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading products: {ex}");
            // You might want to show an error message to the user here
        }
    }

    private async void AddProduct(object? sender, RoutedEventArgs e)
    {
        try
        {
            ContextData.selectedProductInMainWindow = null;
            var window = new GoodsManagementWindow();
            window.Closed += (s, e) => LoadProducts();
            await window.ShowDialog((Window)this.VisualRoot);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding product: {ex}");
        }
    }

    private async void EditProduct(object? sender, RoutedEventArgs e)
    {
        try
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as Product;
            if (selectedProduct == null) return;

            // Load fresh data for the selected product
            var freshProduct = await App.DbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.IdProducts == selectedProduct.IdProducts);

            if (freshProduct == null) return;

            ContextData.selectedProductInMainWindow = freshProduct;
            var window = new GoodsManagementWindow();
            window.Closed += (s, e) => LoadProducts();
            await window.ShowDialog((Window)this.VisualRoot);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error editing product: {ex}");
        }
    }

    private async void DeleteProduct(object? sender, RoutedEventArgs e)
    {
        try
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as Product;
            if (selectedProduct == null) return;

            // Get a fresh instance of the product to delete
            var productToDelete = await App.DbContext.Products
                .FirstOrDefaultAsync(p => p.IdProducts == selectedProduct.IdProducts);

            if (productToDelete != null)
            {
                App.DbContext.Products.Remove(productToDelete);
                await App.DbContext.SaveChangesAsync();
                LoadProducts();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting product: {ex}");
        }
    }

    private void ProductsDataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        EditProduct(sender, e);
    }
}