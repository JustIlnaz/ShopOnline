using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShopOnline.Data;
using ShopOnline.Models;
using System.Linq;

namespace ShopOnline;

public partial class EmployeeManagementUserControl : UserControl
{
    public EmployeeManagementUserControl()
    {
        InitializeComponent();
        App.DbContext.Users.ToList();
        MainDataGrid.ItemsSource = App.DbContext.Logins.ToList();

    }

    private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var selectedLogin = MainDataGrid.SelectedItem as Login;

        if (selectedLogin == null) return;

        ContextData.selectedLogin1InMainWindow = selectedLogin;

        var createAndChangeUserWindow = new EmployeeWindow();
        var parent = this.VisualRoot as Window;
        await createAndChangeUserWindow.ShowDialog(parent);

        MainDataGrid.ItemsSource = App.DbContext.Logins.ToList();
    }



    private async void AddEmployee(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ContextData.selectedLogin1InMainWindow = null;

        var createAndChangeUserWindow = new EmployeeWindow();
        var parent = this.VisualRoot as Window;

        await createAndChangeUserWindow.ShowDialog(parent);
        MainDataGrid.ItemsSource = App.DbContext.Logins.ToList();
    }

    private void DeleteButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var selectedLogin = MainDataGrid.SelectedItem as Login;

        App.DbContext.Logins.Remove(selectedLogin);
        App.DbContext.SaveChanges();

        MainDataGrid.ItemsSource = App.DbContext.Logins.ToList();

    }
}


