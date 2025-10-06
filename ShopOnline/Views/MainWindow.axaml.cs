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

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorTextBlock.Text = "Логин и пароль не могут быть пустыми";
                return;
            }

            // Поиск пользователя по логину
            var userLogin = await App.DbContext.Logins
                 .Include(x => x.User)
                 .ThenInclude(y => y.Role)
                 .FirstOrDefaultAsync(x => EF.Functions.Like(x.Login1, login));

            if (userLogin == null)
            {
                ErrorTextBlock.Text = "Пользователь не найден";
                return;
            }

            // Хэшируем введенный пароль для сравнения
            string hashedPassword = HashPassword(password);

            // Проверка пароля
            if (userLogin.Password?.Trim() != hashedPassword)
            {
                ErrorTextBlock.Text = "Неверный пароль";
                return;
            }

            // Проверка роли
            if (userLogin.User?.Role == null)
            {
                ErrorTextBlock.Text = "Ошибка доступа: роль не назначена";
                return;
            }

            // Открытие соответствующего окна
            string roleName = userLogin.User.Role.NameRole?.Trim() ?? "";
            Window nextWindow = roleName switch
            {
                "Администратор" => new AdminWindow(),
                "Покупатель" => new CustomerWindow(),
                "Менеджер" => new ManagerWindow(),
                _ => new CustomerWindow() // окно по умолчанию
            };

            nextWindow.Show();
            this.Close();
        }

        // Добавляем метод хэширования пароля (такой же как в LoginWindow)
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}