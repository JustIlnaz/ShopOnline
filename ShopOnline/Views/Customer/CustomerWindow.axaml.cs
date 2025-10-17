using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ShopOnline.Views.Customer;

namespace ShopOnline;

public partial class CustomerWindow : Window
{
    private readonly CustomerGoodsUserControl _goodsControl;
    private readonly CustomerBasketUserControl _basketControl;
    private readonly CustomerOrdersUserControl _ordersControl;
 

    public CustomerWindow()
    {
        InitializeComponent();

        // Initialize all controls
        _goodsControl = new CustomerGoodsUserControl();
        _basketControl = new CustomerBasketUserControl();
        _ordersControl = new CustomerOrdersUserControl();
      // Reusing the manager profile control for customers

        // Show goods by default
        var mainContent = this.FindControl<ContentControl>("MainContent");
        if (mainContent != null)
        {
            mainContent.Content = _goodsControl;
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

    private void OnGoodsClick(object sender, RoutedEventArgs e)
    {
        NavigateTo(_goodsControl);
    }

    private void OnBasketClick(object sender, RoutedEventArgs e)
    {
        NavigateTo(_basketControl);
    }

    private void OnOrdersClick(object sender, RoutedEventArgs e)
    {
        NavigateTo(_ordersControl);
    }

   
}