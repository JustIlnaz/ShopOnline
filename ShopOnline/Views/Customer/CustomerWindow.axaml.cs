using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShopOnline.Data;
using ShopOnline.Models;
using System;

namespace ShopOnline.Views.Customer;

public partial class CustomerWindow : Window
{
    private readonly CustomerGoodsUserControl _goodsControl;
    private readonly CustomerBasketUserControl _basketControl;
    private readonly CustomerOrdersUserControl _ordersControl;


    public CustomerWindow()
    {
        InitializeComponent();

        // Validate user session
        if (ContextData.CurrentUser == null || ContextData.selectedLogin1InMainWindow == null)
        {
            ShowError("������ ������ ������������");
            this.Close();
            new MainWindow().Show();
            return;
        }

        // Initialize all controls
        _goodsControl = new CustomerGoodsUserControl();
        _basketControl = new CustomerBasketUserControl();
        _ordersControl = new CustomerOrdersUserControl();

        // Load initial content
        try
        {
            Goods_Click(null, null);
        }
        catch (Exception ex)
        {
            ShowError($"������ ��� ��������: {ex.Message}");
        }
    }

    private void NavigateTo(UserControl control)
    {
        var mainContent = this.FindControl<ContentControl>("MainContent");
        if (mainContent != null)
        {
            mainContent.Content = control;
        }
    }

    private void Goods_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new CustomerGoodsUserControl();
        //NavigateTo(_goodsControl);
    }

    private void Basket_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new CustomerBasketUserControl();
        //NavigateTo(_basketControl);
    }

    private void Orders_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new CustomerOrdersUserControl();
        // NavigateTo(_ordersControl);
    }

    private void ProfileClick(object? sender, RoutedEventArgs e)
    {
        ContentArea.Content = new CustomerProfileUserControl();
        // NavigateTo(_profileControl);
    }

    private void LogoutClick(object? sender, RoutedEventArgs e)
    {
        // Clear session data
        ContextData.CurrentUser = null;
        ContextData.selectedLogin1InMainWindow = null;
        ContextData.selectedBasketInMainWindow = null;
        ContextData.selectedRoleInMainWindow = null;

        // Show login window
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

    private void ShowError(string message)
    {
        var messageBox = new Window
        {
            Title = "������",
            Content = new TextBlock { Text = message },
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        messageBox.Show();
    }

    private void MenuItem_Click(object? sender, RoutedEventArgs e)
    {
    }
}