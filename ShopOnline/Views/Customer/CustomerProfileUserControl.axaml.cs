using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Linq;

namespace ShopOnline.Views.Customer;

public partial class CustomerProfileUserControl : UserControl
{
    private User? _currentUser;
    private User? _originalUser;

    public CustomerProfileUserControl()
    {
        InitializeComponent();
        LoadUserProfile();
    }

    private void LoadUserProfile()
    {
        try
        {
            if (ContextData.CurrentUser == null)
            {
                ShowMessage("Пользователь не авторизован", "Ошибка");
                return;
            }

            // Load user with role information
            _currentUser = App.DbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.IdUsers == ContextData.CurrentUser.IdUsers);

            if (_currentUser == null)
            {
                ShowMessage("Профиль пользователя не найден", "Ошибка");
                return;
            }

            // Create a copy for editing
            _originalUser = new User
            {
                IdUsers = _currentUser.IdUsers,
                FullName = _currentUser.FullName,
                Email = _currentUser.Email,
                PhoneNumber = _currentUser.PhoneNumber,
                Description = _currentUser.Description,
                RoleId = _currentUser.RoleId,
                Role = _currentUser.Role
            };

            // Bind data to controls
            FullNameTextBox.Text = _currentUser.FullName ?? "";
            EmailTextBox.Text = _currentUser.Email ?? "";
            PhoneTextBox.Text = _currentUser.PhoneNumber ?? "";
            DescriptionTextBox.Text = _currentUser.Description ?? "";
            RoleTextBox.Text = _currentUser.Role?.NameRole ?? "";

            ClearStatus();
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при загрузке профиля: {ex.Message}", "Ошибка");
        }
    }

    private void SaveProfile(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_currentUser == null)
            {
                ShowMessage("Профиль не загружен", "Ошибка");
                return;
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                ShowStatus("Поле 'Полное имя' обязательно для заполнения", "Red");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowStatus("Поле 'Email' обязательно для заполнения", "Red");
                return;
            }

            // Validate email format
            if (!IsValidEmail(EmailTextBox.Text))
            {
                ShowStatus("Неверный формат email адреса", "Red");
                return;
            }

            // Check if email is already used by another user
            var existingUser = App.DbContext.Users
                .FirstOrDefault(u => u.Email == EmailTextBox.Text && u.IdUsers != _currentUser.IdUsers);
            
            if (existingUser != null)
            {
                ShowStatus("Email адрес уже используется другим пользователем", "Red");
                return;
            }

            // Update user data
            _currentUser.FullName = FullNameTextBox.Text.Trim();
            _currentUser.Email = EmailTextBox.Text.Trim();
            _currentUser.PhoneNumber = PhoneTextBox.Text?.Trim();
            _currentUser.Description = DescriptionTextBox.Text?.Trim();

            App.DbContext.SaveChanges();

            // Update context data
            ContextData.CurrentUser = _currentUser;

            ShowStatus("Профиль успешно сохранен", "Green");
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при сохранении профиля: {ex.Message}", "Ошибка");
        }
    }

    private void CancelChanges(object? sender, RoutedEventArgs e)
    {
        if (_originalUser != null)
        {
            FullNameTextBox.Text = _originalUser.FullName ?? "";
            EmailTextBox.Text = _originalUser.Email ?? "";
            PhoneTextBox.Text = _originalUser.PhoneNumber ?? "";
            DescriptionTextBox.Text = _originalUser.Description ?? "";
        }
        ClearStatus();
    }

    private void ResetPassword(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_currentUser == null)
            {
                ShowMessage("Профиль не загружен", "Ошибка");
                return;
            }

            // Find user's login
            var login = App.DbContext.Logins
                .FirstOrDefault(l => l.UserId == _currentUser.IdUsers);

            if (login == null)
            {
                ShowMessage("Данные для входа не найдены", "Ошибка");
                return;
            }

            // Generate new password (simple implementation)
            var newPassword = GenerateNewPassword();
            login.Password = newPassword; // In real app, this should be hashed

            App.DbContext.SaveChanges();

            ShowMessage($"Новый пароль: {newPassword}\n\nСохраните его в безопасном месте!", "Новый пароль");
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при сбросе пароля: {ex.Message}", "Ошибка");
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateNewPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private void ShowStatus(string message, string color)
    {
        StatusText.Text = message;
        StatusText.Foreground = color == "Red" ? Avalonia.Media.Brushes.Red : Avalonia.Media.Brushes.Green;
    }

    private void ClearStatus()
    {
        StatusText.Text = "";
    }

    private void ShowMessage(string message, string title)
    {
        var messageBox = new Window
        {
            Title = title,
            Content = new TextBlock { Text = message, Margin = new Avalonia.Thickness(10) },
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        messageBox.Show();
    }
}
