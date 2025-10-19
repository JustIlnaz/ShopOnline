using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShopOnline.Data;
using ShopOnline.Models;
using ShopOnline.Views;

namespace ShopOnline;

public partial class AdminWindow : Window
{
    public AdminWindow()
    {
        InitializeComponent();
        Profile_Click(null, null);
    }

    private void Profile_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new AdminProfileUserControl();
    }

    private void Goods_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new AdminGoodsUserControl();
    }

    private void Purchases_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new AdminPurchasesUserControl();
    }

    private void CustomerManagment_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new CustomerManagementUserControl();
    }

    private void EmployeeManagment_Click(object? sender, RoutedEventArgs? e)
    {
        ContentArea.Content = new EmployeeManagementUserControl();
    }

    private void LogoutClick(object? sender, RoutedEventArgs e)
    {
        // Clear the current user data
        ContextData.CurrentUser = null;
        ContextData.selectedLogin1InMainWindow = null;

        // Create and show the main window
        var mainWindow = new MainWindow();
        mainWindow.Show();

        // Close the current window
        this.Close();
    }
}