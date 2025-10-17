using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShopOnline.Views.Manager;

namespace ShopOnline;

public partial class ManagerWindow : Window
{
    private readonly ManagerCustomerManagementUserControl _customerManagementControl;
    private readonly ManagerGoodsUserControl _goodsManagementControl;
    private readonly ManagerPurchasesUserControl _purchasesControl;
    private readonly ManagerProfileUserControl _profileControl;

    public ManagerWindow()
    {
        InitializeComponent();
        
        _customerManagementControl = new ManagerCustomerManagementUserControl();
        _goodsManagementControl = new ManagerGoodsUserControl();
        _purchasesControl = new ManagerPurchasesUserControl();
        _profileControl = new ManagerProfileUserControl();

        // Show customer management by default
        MainContent.Content = _customerManagementControl;
    }

    private void OnCustomerManagementClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _customerManagementControl;
    }

    private void OnGoodsManagementClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _goodsManagementControl;
    }

    private void OnUserPurchasesClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _purchasesControl;
    }

    private void OnProfileEditClick(object? sender, RoutedEventArgs e)
    {
        MainContent.Content = _profileControl;
    }
}