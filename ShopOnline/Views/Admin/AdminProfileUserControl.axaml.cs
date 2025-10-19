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

public partial class AdminProfileUserControl : UserControl
{
    private Login? _currentLogin;
    private Login? _originalLogin;

    public AdminProfileUserControl()
    {
        InitializeComponent();
        LoadProfile();
    }

    private void LoadProfile()
    {
        try
        {
            var context = App.DbContext;
            
            // First try to load from selected login if available
            if (ContextData.selectedLogin1InMainWindow != null)
            {
                var selectedId = ContextData.selectedLogin1InMainWindow.IdLogins;
                _currentLogin = context.Logins
                    .Include(l => l.User)
                    .ThenInclude(u => u.Role)
                    .AsNoTracking() // Important: Get a clean copy without tracking
                    .Where(l => l.IdLogins == selectedId)
                    .FirstOrDefault();
            }
            
            // If no selected login or not found, try to load from current user
            if (_currentLogin == null && ContextData.CurrentUser != null)
            {
                var currentUserId = ContextData.CurrentUser.IdUsers;
                _currentLogin = context.Logins
                    .Include(l => l.User)
                    .ThenInclude(u => u.Role)
                    .AsNoTracking() // Important: Get a clean copy without tracking
                    .Where(l => l.User.IdUsers == currentUserId)
                    .FirstOrDefault();

                if (_currentLogin == null)
                {
                    // Try to reload the current user with full details
                    var fullUser = context.Users
                        .Include(u => u.Logins)
                        .Include(u => u.Role)
                        .AsNoTracking() // Important: Get a clean copy without tracking
                        .FirstOrDefault(u => u.IdUsers == currentUserId);

                    if (fullUser?.Logins != null && fullUser.Logins.Any())
                    {
                        _currentLogin = fullUser.Logins.First();
                    }
                }
            }

            if (_currentLogin != null)
            {
                // Store original data for reset functionality
                _originalLogin = Clone(_currentLogin);
                
                // Set the DataContext
                DataContext = _currentLogin;
                ShowMessage("Профиль успешно загружен");
            }
            else
            {
                ShowError("Не удалось загрузить данные профиля. Убедитесь, что вы вошли в систему.");
                System.Diagnostics.Debug.WriteLine($"Failed to load profile. CurrentUser ID: {ContextData.CurrentUser?.IdUsers}, Selected Login ID: {ContextData.selectedLogin1InMainWindow?.IdLogins}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка загрузки профиля: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Profile loading error: {ex}");
        }
    }

    private Login Clone(Login source)
    {
        return new Login
        {
            IdLogins = source.IdLogins,
            Login1 = source.Login1,
            Password = source.Password,
            User = new User
            {
                IdUsers = source.User.IdUsers,
                FullName = source.User.FullName,
                Email = source.User.Email,
                PhoneNumber = source.User.PhoneNumber,
                Description = source.User.Description,
                Role = source.User.Role
            }
        };
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_currentLogin == null)
            {
                ShowError("Нет данных для сохранения");
                return;
            }

            var context = App.DbContext;
            
            // Validate required fields
            if (string.IsNullOrWhiteSpace(_currentLogin.Login1) ||
                string.IsNullOrWhiteSpace(_currentLogin.Password) ||
                string.IsNullOrWhiteSpace(_currentLogin.User.FullName) ||
                string.IsNullOrWhiteSpace(_currentLogin.User.Email) ||
                string.IsNullOrWhiteSpace(_currentLogin.User.PhoneNumber))
            {
                ShowError("Все поля кроме описания обязательны для заполнения");
                return;
            }

            // Check if login exists (excluding current user)
            var existingLogin = await context.Logins
                .FirstOrDefaultAsync(l => l.Login1 == _currentLogin.Login1 && l.IdLogins != _currentLogin.IdLogins);

            if (existingLogin != null)
            {
                ShowError("Этот логин уже используется");
                return;
            }

            // Get the entity to update with tracking enabled
            var entityToUpdate = await context.Logins
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.IdLogins == _currentLogin.IdLogins);

            if (entityToUpdate != null)
            {
                // Update the entity properties
                entityToUpdate.Login1 = _currentLogin.Login1;
                entityToUpdate.Password = _currentLogin.Password;
                entityToUpdate.User.FullName = _currentLogin.User.FullName;
                entityToUpdate.User.Email = _currentLogin.User.Email;
                entityToUpdate.User.PhoneNumber = _currentLogin.User.PhoneNumber;
                entityToUpdate.User.Description = _currentLogin.User.Description;

                await context.SaveChangesAsync();
                
                // Update the original data after successful save
                _originalLogin = Clone(_currentLogin);
                
                ShowMessage("Профиль успешно обновлен");
            }
            else
            {
                ShowError("Не удалось найти профиль для обновления");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при сохранении изменений: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Save error: {ex}");
        }
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        if (_originalLogin != null)
        {
            // Reset to original data
            _currentLogin = Clone(_originalLogin);
            DataContext = _currentLogin;
            ShowMessage("Изменения отменены");
        }
        else
        {
            // If no original data available, reload from database
            LoadProfile();
        }
    }

    private void ShowError(string message)
    {
        var validationGrid = this.FindControl<Grid>("ValidationErrors");
        var validationMessage = this.FindControl<TextBlock>("ValidationMessage");
        if (validationGrid != null && validationMessage != null)
        {
            validationGrid.IsVisible = true;
            validationMessage.Text = message;
            validationMessage.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#D32F2F"));
        }
    }

    private void ShowMessage(string message)
    {
        var validationGrid = this.FindControl<Grid>("ValidationErrors");
        var validationMessage = this.FindControl<TextBlock>("ValidationMessage");
        if (validationGrid != null && validationMessage != null)
        {
            validationGrid.IsVisible = true;
            validationMessage.Text = message;
            validationMessage.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#2E7D32"));
        }
    }
}