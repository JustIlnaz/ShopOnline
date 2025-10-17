using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System.Linq;

namespace ShopOnline.Views.Manager
{
    public partial class ManagerCustomerManagementUserControl : UserControl
    {
        public ManagerCustomerManagementUserControl()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var customers = App.DbContext.Users
                .Include(u => u.Role)
                .Where(u => EF.Functions.Like(u.Role.NameRole, "Покупатель"))
                .ToList();
            CustomersGrid.ItemsSource = customers;
        }

        private void OnSearch(object? sender, RoutedEventArgs e)
        {
            var searchText = SearchBox.Text ?? "";
            var filteredCustomers = App.DbContext.Users
                .Include(u => u.Role)
                .Where(u => EF.Functions.Like(u.Role.NameRole, "Покупатель") &&
                           (EF.Functions.Like(u.FullName, $"%{searchText}%") ||
                            EF.Functions.Like(u.Email, $"%{searchText}%") ||
                            EF.Functions.Like(u.PhoneNumber, $"%{searchText}%")))
                .ToList();
            CustomersGrid.ItemsSource = filteredCustomers;
        }
    }
}