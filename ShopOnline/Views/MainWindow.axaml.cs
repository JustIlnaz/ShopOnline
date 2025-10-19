using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using ShopOnline.Views.Customer;
using System;
using System.Linq;

namespace ShopOnline.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var newLoginWindow = new LoginWindow();
            newLoginWindow.Show();
        }

        private async void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                string login = LoginTB?.Text?.Trim() ?? "";
                string password = PasswordTextBox?.Text?.Trim() ?? "";

                // �������� �� ������ ����
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    ErrorTextBlock.Text = "����� � ������ �� ����� ���� �������";
                    return;
                }

                // ����� ������������ �� ������ �� ����� ������������ �������
                var userLogin = await App.DbContext.Logins
                    .Include(x => x.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(x => x.Login1.Trim().ToLower() == login.ToLower());

                if (userLogin == null || userLogin.User == null)
                {
                    ErrorTextBlock.Text = "������������ �� ������";
                    return;
                }

                // �������� ������
                if (string.IsNullOrEmpty(userLogin.Password) || userLogin.Password.Trim() != password)
                {
                    ErrorTextBlock.Text = "�������� ������";
                    return;
                }

                // �������� ����
                if (userLogin.User.Role == null)
                {
                    ErrorTextBlock.Text = "������ �������: ���� �� ���������";
                    return;
                }

                // Store login and user data in ContextData
                ContextData.selectedLogin1InMainWindow = userLogin;
                ContextData.CurrentUser = userLogin.User;
                ContextData.selectedRoleInMainWindow = userLogin.User.Role;

                // Ensure basket exists for customer
                if (userLogin.User.Role.NameRole?.Trim() == "����������")
                {
                    var basket = await App.DbContext.Baskets
                        .FirstOrDefaultAsync(b => b.UsersId == userLogin.User.IdUsers);

                    if (basket == null)
                    {
                        basket = new Basket { UsersId = userLogin.User.IdUsers };
                        App.DbContext.Baskets.Add(basket);
                        await App.DbContext.SaveChangesAsync();
                    }
                    ContextData.selectedBasketInMainWindow = basket;
                }

                // ����������� ���������� ���� �� ����
                string roleName = userLogin.User.Role.NameRole?.Trim() ?? "";
                Window nextWindow;
                
                switch (roleName.ToLower())
                {
                    case "�������������":
                        nextWindow = new AdminWindow();
                        break;
                    case "����������":
                        nextWindow = new CustomerWindow();
                        break;
                    case "��������":
                        nextWindow = new ManagerWindow();
                        break;
                    default:
                        ErrorTextBlock.Text = "����������� ���� ������������";
                        return;
                }

                nextWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"��������� ������ ��� �����: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
            }
        }
    }
}