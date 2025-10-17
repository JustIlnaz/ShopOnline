using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Linq;

namespace ShopOnline;

public partial class CustomerManagementUserControl : UserControl
{
    private DataGrid? customersDataGrid;

    public CustomerManagementUserControl()
    {
        InitializeComponent();
        App.DbContext.Users.ToList();
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        MainDataGrid.ItemsSource = App.DbContext.Logins
            .Include(l => l.User)
            .Include(l => l.User.Role)
            .Where(l => EF.Functions.Like(l.User.Role.NameRole, "Покупатель") )
            .ToList();
    }

    private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var selectedLogin = MainDataGrid.SelectedItem as Login;

        if (selectedLogin == null) return;

        ContextData.selectedLogin1InMainWindow = selectedLogin;

        var createAndChangeUserWindow = new  CustomerManagementWindow();
        var parent = this.VisualRoot as Window;
        await createAndChangeUserWindow.ShowDialog(parent);

        RefreshGrid();
    }

    private async void AddCustomer(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ContextData.selectedLogin1InMainWindow = null;

        var createAndChangeUserWindow = new CustomerManagementWindow();
        var parent = this.VisualRoot as Window;

        await createAndChangeUserWindow.ShowDialog(parent);

        RefreshGrid();
    }

    private void DeleteButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var selectedLogin = MainDataGrid.SelectedItem as Login;

        if (selectedLogin != null)
        {
            App.DbContext.Logins.Remove(selectedLogin);
            App.DbContext.SaveChanges();

            RefreshGrid();
        }
    }
}