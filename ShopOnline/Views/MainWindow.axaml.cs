using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShopOnline.Data;
using System;
using System.Linq;

namespace ShopOnline.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.DbContext.Logins.ToList();
            App.DbContext.Users.ToList();
            App.DbContext.Roles.ToList();
        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var newLoginWindow = new LoginWindow();
            newLoginWindow.Show();
        }

        private async void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string login = LoginTB.Text?.Trim() ?? "";
            string password = PasswordTextBox.Text?.Trim() ?? "";

            // �������� �� ������ ����
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorTextBlock.Text = "����� � ������ �� ����� ���� �������";
                return;
            }

            // ����� ������������ �� ������
            var userLogin = await App.DbContext.Logins
                 .Include(x => x.User)
                 .ThenInclude(y => y.Role)
                 .FirstOrDefaultAsync(x => EF.Functions.Like(x.Login1, login));

            if (userLogin == null)
            {
                ErrorTextBlock.Text = "������������ �� ������";
                return;
            }

            // �������� ��������� ������ ��� ���������
            string Password = password;

            // �������� ������
            if (userLogin.Password?.Trim() != Password)
            {
                ErrorTextBlock.Text = "�������� ������";
                return;
            }

            // �������� ����
            if (userLogin.User?.Role == null)
            {
                ErrorTextBlock.Text = "������ �������: ���� �� ���������";
                return;
            }

            // �������� ���������������� ����
            string roleName = userLogin.User.Role.NameRole?.Trim() ?? "";
            Window nextWindow = roleName switch
            {
                "�������������" => new AdminWindow(),
                "����������" => new CustomerWindow(),
                "��������" => new ManagerWindow(),
                _ => new CustomerWindow() // ���� �� ���������
            };

            nextWindow.Show();
            this.Close();
        }

    }
}