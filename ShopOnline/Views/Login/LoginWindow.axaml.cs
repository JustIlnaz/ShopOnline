using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopOnline;

public partial class LoginWindow : Window
{

    public LoginWindow()
    {
        InitializeComponent();
        App.DbContext.Logins.ToList();
    }

    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string fullName = FullNameText.Text?.Trim() ?? "";
        string phoneNumber = PhoneNumberText.Text?.Trim() ?? "";
        string description = DescriptionText.Text?.Trim();
        string login = LoginText.Text?.Trim() ?? "";
        string password = PasswordText.Text?.Trim() ?? "";

        string inputLogin = LoginText.Text?.Trim();
        string inputPassword = PasswordText.Text?.Trim();

       
        // Валидация
        if (string.IsNullOrEmpty(fullName))
        {
            return;
        }
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return;
        }
        if (string.IsNullOrEmpty(login))
        {
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            return;
        }

    
        // Проверка на уникальность логина 
        bool loginExists = await App.DbContext.Logins.AnyAsync(u => u.Login1 == login);
        if (loginExists)
        {
            return;
        }
        var user = new User
        {
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Description = description,
            RoleId = 3
        };
        App.DbContext.Users.Add(user);
        await App.DbContext.SaveChangesAsync();

        var loginEntity = new Login
        {
            Login1 = login.ToString(),
            Password = HashPassword(password), // Хэшируем пароль!
            UserId = user.IdUsers
        };




        App.DbContext.Logins.Add(loginEntity);
        await App.DbContext.SaveChangesAsync();
        this.Close();
    }



    private string HashPassword(string password)
    {
      
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

}
