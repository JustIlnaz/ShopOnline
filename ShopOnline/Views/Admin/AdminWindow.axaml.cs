using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShopOnline;

public partial class AdminWindow : Window
{
    public AdminWindow()
    {
        InitializeComponent();
    }

    private void NavigateToEmployeeManagement(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainContentArea.Content = new EmployeeManagementUserControl();
    }

    private void NavigateToCustomerManagement(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainContentArea.Content = new CustomerManagementUserControl();
    }

    private void NavigateToAdminProfile(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainContentArea.Content = new AdminProfileUserControl();
    }

    private void NavigateToPurchases(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainContentArea.Content = new AdminPurchasesUserControl();
    }

    private void NavigateToGoods(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainContentArea.Content = new AdminGoodsUserControl();
    }
}