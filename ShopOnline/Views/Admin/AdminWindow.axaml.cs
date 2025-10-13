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
        AdminEmployee.Content = new EmployeeManagementUserControl();
    }

    private void NavigateToAdminProfile(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AdminProfile.Content = new AdminProfileUserControl();
    }

    private void NavigateToPurchases(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AdminPurchases.Content = new AdminPurchasesUserControl();
    }

    private void NavigateToGoods(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AdminGoods.Content = new AdminGoodsUserControl(); 
    }
}