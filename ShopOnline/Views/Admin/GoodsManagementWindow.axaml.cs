using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShopOnline.Data;
using System.Linq;
using System;
using ShopOnline.Models;

namespace ShopOnline;

public partial class GoodsManagementWindow : Window
{
 
    public GoodsManagementWindow()
    {
        InitializeComponent();
        LoadCategories();

        if (ContextData.selectedProductInMainWindow == null) return;
        LoadProductData();
    }

    private void LoadCategories()
    {
        var categories = App.DbContext.Categories.ToList();
        CategoryComboBox.ItemsSource = categories;
        CategoryComboBox.DisplayMemberBinding = new Avalonia.Data.Binding("NameCategories");
    }

    private void LoadProductData()
    {
        var product = ContextData.selectedProductInMainWindow;
        if (product == null) return;

        NameProductText.Text = product.NameProducts;
        DescriptionText.Text = product.Description;
        PriceText.Text = product.Price;
        CountText.Text = product.CountInSklade.ToString();

        var category = App.DbContext.Categories.FirstOrDefault(c => c.IdCategories == product.CategoryId);
        if (category != null)
        {
            CategoryComboBox.SelectedItem = category;
        }
    }

    private void SaveButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(NameProductText.Text)) return;
        if (string.IsNullOrEmpty(DescriptionText.Text)) return;
        if (string.IsNullOrEmpty(PriceText.Text)) return;
        if (string.IsNullOrEmpty(CountText.Text)) return;
        if (CategoryComboBox.SelectedItem == null) return;

        var selectedCategory = CategoryComboBox.SelectedItem as Category;
        if (selectedCategory == null) return;

        if (ContextData.selectedProductInMainWindow != null)
        {
            var product = App.DbContext.Products.FirstOrDefault(x => x.IdProducts == ContextData.selectedProductInMainWindow.IdProducts);
            if (product == null) return;

            UpdateProduct(product, selectedCategory);
        }
        else
        {
            var newProduct = new Product();
            UpdateProduct(newProduct, selectedCategory);
            App.DbContext.Products.Add(newProduct);
        }

        App.DbContext.SaveChanges();
        this.Close();
    }

    private void UpdateProduct(Product product, Category category)
    {
        product.NameProducts = NameProductText.Text;
        product.Description = DescriptionText.Text;
        product.Price = PriceText.Text;
        product.CountInSklade = int.Parse(CountText.Text);
        product.CategoryId = category.IdCategories;
        product.Category = category;
        product.DateSave = DateTime.Now;
    }
}