using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using ShopOnline.Data;
using System;
using System.Linq;

namespace ShopOnline.Views.Manager;

public partial class EditProductWindow : Window
{
    private readonly Product? _product;
    private bool _isNewProduct;

    public EditProductWindow()
    {
        InitializeComponent();
        _isNewProduct = true;
        LoadCategories();
    }

    public EditProductWindow(Product product)
    {
        InitializeComponent();
        _product = product;
        _isNewProduct = false;
        LoadCategories();
        LoadProductData();
    }

    private void LoadCategories()
    {
        var categories = App.DbContext.Categories.ToList();
        CategoryComboBox.ItemsSource = categories;
        CategoryComboBox.DisplayMemberBinding = new Binding("NameCategories");
    }

    private void LoadProductData()
    {
        if (_product != null)
        {
            NameBox.Text = _product.NameProducts;
            DescriptionBox.Text = _product.Description;
            PriceBox.Text = _product.Price;
            CountBox.Text = _product.CountInSklade?.ToString() ?? "0";
            CategoryComboBox.SelectedItem = CategoryComboBox.ItemsSource.Cast<Category>()
                .FirstOrDefault(c => c.IdCategories == _product.CategoryId);
        }
    }

    private async void OnSave(object? sender, RoutedEventArgs e)
    {
        if (!ValidateInput())
        {
            var messageBox = new Window
            {
                Title = "Ошибка",
                Content = new TextBlock
                {
                    Text = "Пожалуйста, заполните все поля корректно:\n" +
                           "- Название не должно быть пустым\n" +
                           "- Цена должна быть числом\n" +
                           "- Количество должно быть числом\n" +
                           "- Категория должна быть выбрана",
                    Margin = new Thickness(20)
                },
                SizeToContent = SizeToContent.WidthAndHeight,
                Width = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await messageBox.ShowDialog(this);
            return;
        }

        if (_isNewProduct)
        {
            var newProduct = new Product
            {
                NameProducts = NameBox.Text,
                Description = DescriptionBox.Text ?? "",
                Price = PriceBox.Text,
                CountInSklade = int.Parse(CountBox.Text),
                CategoryId = ((Category)CategoryComboBox.SelectedItem!).IdCategories,
                DateSave = DateTime.Now
            };
            App.DbContext.Products.Add(newProduct);
        }
        else if (_product != null)
        {
            _product.NameProducts = NameBox.Text;
            _product.Description = DescriptionBox.Text ?? "";
            _product.Price = PriceBox.Text;
            _product.CountInSklade = int.Parse(CountBox.Text);
            _product.CategoryId = ((Category)CategoryComboBox.SelectedItem!).IdCategories;
            _product.DateSave = DateTime.Now;
            App.DbContext.Products.Update(_product);
        }

        App.DbContext.SaveChanges();
        Close();
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
            return false;

        if (!int.TryParse(CountBox.Text, out _))
            return false;

        if (!int.TryParse(PriceBox.Text, out _))
            return false;

        if (CategoryComboBox.SelectedItem == null)
            return false;

        return true;
    }
}