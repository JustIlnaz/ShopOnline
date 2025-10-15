using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Linq;
using Avalonia.Interactivity;

namespace ShopOnline;

public partial class AdminProfileUserControl : UserControl
{
    private Login? _currentLogin;

    public AdminProfileUserControl()
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
                    .FirstOrDefault();
            }

            if (_currentLogin != null)
            {
                DataContext = _currentLogin;
            }
        }
        catch (Exception ex)
        {
            ShowError("Failed to load profile data: " + ex.Message);
        }
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_currentLogin == null)
            {
                ShowError("No data to save");
                return;
            }

            var existingLogin = await App.DbContext.Logins
                .FirstOrDefaultAsync(l => l.Login1 == _currentLogin.Login1 && l.IdLogins != _currentLogin.IdLogins);
            
            if (existingLogin != null)
            {
                ShowError("This login is already taken");
                return;
            }

            App.DbContext.Logins.Update(_currentLogin);
            await App.DbContext.SaveChangesAsync();
            ShowMessage("Profile updated successfully");
        }
        catch (Exception ex)
        {
            ShowError("Failed to save changes: " + ex.Message);
        }
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        LoadProfile();
    }

    private void ShowError(string message)
    {
    
    }

    private void ShowMessage(string message)
    {
    }
}