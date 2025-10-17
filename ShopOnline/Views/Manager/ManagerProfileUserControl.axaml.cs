using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Linq;

namespace ShopOnline.Views.Manager;

public partial class ManagerProfileUserControl : UserControl
{
    private Login? _currentLogin;

    public ManagerProfileUserControl()
    {
        InitializeComponent();
        LoadProfile();
    }

    private void LoadProfile()
    {
        try
        {
            if (ContextData.selectedLogin1InMainWindow != null)
            {
                var selectedId = ContextData.selectedLogin1InMainWindow.IdLogins;
                _currentLogin = App.DbContext.Logins
                    .Include(l => l.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefault(l => l.IdLogins == selectedId);
            }
            else
            {
                _currentLogin = App.DbContext.Logins
                    .Include(l => l.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefault(l => l.User.IdUsers == ContextData.CurrentUser.IdUsers);
            }

            if (_currentLogin != null)
            {
                DataContext = _currentLogin;
            }
            else
            {
                ShowError("Не удалось загрузить данные профиля");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при загрузке профиля: {ex.Message}");
        }
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

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(_currentLogin.Login1) ||
                string.IsNullOrWhiteSpace(_currentLogin.Password) ||
                string.IsNullOrWhiteSpace(_currentLogin.User.FullName) ||
                string.IsNullOrWhiteSpace(_currentLogin.User.Email) ||
                string.IsNullOrWhiteSpace(_currentLogin.User.PhoneNumber))
            {
                ShowError("Все поля кроме описания обязательны для заполнения");
                return;
            }

            // Проверка существования логина
            var existingLogin = await App.DbContext.Logins
                .FirstOrDefaultAsync(l => l.Login1 == _currentLogin.Login1 && l.IdLogins != _currentLogin.IdLogins);

            if (existingLogin != null)
            {
                ShowError("Этот логин уже используется");
                return;
            }

            App.DbContext.Logins.Update(_currentLogin);
            await App.DbContext.SaveChangesAsync();
            ShowMessage("Профиль успешно обновлен");
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка при сохранении изменений: {ex.Message}");
        }
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        LoadProfile();
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