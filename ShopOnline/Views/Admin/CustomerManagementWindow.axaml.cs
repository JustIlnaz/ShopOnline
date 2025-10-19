using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShopOnline.Data;
using ShopOnline.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace ShopOnline;

public partial class CustomerManagementWindow : Window
{
    public CustomerManagementWindow()
    {
        InitializeComponent();
        InitializeDataContext();
    }

    private void InitializeDataContext()
    {
        try
        {
            if (ContextData.selectedLogin1InMainWindow != null)
            {
                // Load fresh data for editing existing customer
                var freshLogin = App.DbContext.Logins
                    .Include(l => l.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefault(l => l.IdLogins == ContextData.selectedLogin1InMainWindow.IdLogins);

                if (freshLogin != null)
                {
                    DataContext = freshLogin;
                }
            }
            else
            {
                // Initialize new customer
                var dataContextLogin = new Login 
                { 
                    User = new User() 
                };

                // Get customer role
                var customerRole = App.DbContext.Roles
                    .FirstOrDefault(r => r.NameRole == "Покупатель");

                if (customerRole != null)
                {
                    dataContextLogin.User.Role = customerRole;
                    dataContextLogin.User.RoleId = customerRole.IdRoles;
                }

                DataContext = dataContextLogin;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing data context: {ex}");
        }
    }

    private async void SaveButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            // Validate input
            if (string.IsNullOrEmpty(FullNameText?.Text) ||
                string.IsNullOrEmpty(PhoneNumberText?.Text) ||
                string.IsNullOrEmpty(DescriptionText?.Text) ||
                string.IsNullOrEmpty(LoginText?.Text) ||
                string.IsNullOrEmpty(PasswordText?.Text) ||
                string.IsNullOrEmpty(EmailText?.Text))
            {
                // You might want to show an error message to the user here
                return;
            }

            var currentLogin = DataContext as Login;
            if (currentLogin == null) return;

            if (ContextData.selectedLogin1InMainWindow != null)
            {
                // Update existing customer
                var existingLogin = await App.DbContext.Logins
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(x => x.IdLogins == ContextData.selectedLogin1InMainWindow.IdLogins);

                if (existingLogin != null)
                {
                    // Update login details
                    existingLogin.Login1 = currentLogin.Login1;
                    existingLogin.Password = currentLogin.Password;

                    // Update user details
                    existingLogin.User.FullName = currentLogin.User.FullName;
                    existingLogin.User.PhoneNumber = currentLogin.User.PhoneNumber;
                    existingLogin.User.Description = currentLogin.User.Description;
                    existingLogin.User.Email = currentLogin.User.Email;

                    await App.DbContext.SaveChangesAsync();
                }
            }
            else
            {
                // Add new customer
                App.DbContext.Logins.Add(currentLogin);
                await App.DbContext.SaveChangesAsync();
            }

            Close();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving changes: {ex}");
            // You might want to show an error message to the user here
        }
    }
}