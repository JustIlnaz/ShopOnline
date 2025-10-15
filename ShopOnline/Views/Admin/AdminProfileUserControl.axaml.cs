using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Linq;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;

namespace ShopOnline;

public partial class AdminProfileUserControl : UserControl
{
    private AppDbContext? _db;
    private Login? _trackedLogin;
    private Login? _originalLogin;

    // UI Elements
    private TextBlock? _fullNameValidation;
    private TextBlock? _phoneValidation;
    private TextBlock? _emailValidation;
    private TextBlock? _loginValidation;
    private TextBlock? _passwordValidation;

    public AdminProfileUserControl()
    {
        InitializeComponent();
        InitializeValidationControls();
        LoadProfile();
    }

    private void InitializeValidationControls()
    {
        _fullNameValidation = this.FindControl<TextBlock>("FullNameValidation");
        _phoneValidation = this.FindControl<TextBlock>("PhoneValidation");
        _emailValidation = this.FindControl<TextBlock>("EmailValidation");
        _loginValidation = this.FindControl<TextBlock>("LoginValidation");
        _passwordValidation = this.FindControl<TextBlock>("PasswordValidation");
    }

    private void LoadProfile()
    {
        try
        {
            _db?.Dispose();
            _db = new AppDbContext();

            Login? login;
            if (ContextData.selectedLogin1InMainWindow != null)
            {
                var selectedId = ContextData.selectedLogin1InMainWindow.IdLogins;
                login = _db.Logins
                    .Include(l => l.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefault(l => l.IdLogins == selectedId);
            }
            else
            {
                login = _db.Logins
                    .Include(l => l.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefault();
            }

            if (login == null)
            {
                return;
            }

            _trackedLogin = login;
            // Store original state for reset functionality
            _originalLogin = new Login
            {
                IdLogins = login.IdLogins,
                Login1 = login.Login1,
                Password = login.Password,
                User = new User
                {
                    FullName = login.User.FullName,
                    PhoneNumber = login.User.PhoneNumber,
                    Email = login.User.Email,
                    Description = login.User.Description,
                    Role = login.User.Role
                }
            };

            DataContext = _trackedLogin;
        }
        catch (Exception ex)
        {
            // Handle any database connection errors
            ShowError("Failed to load profile data: " + ex.Message);
        }
    }

    private bool ValidateInputs()
    {
        bool isValid = true;

        // Clear previous validation messages
        ClearValidationMessages();

        // Validate Full Name
        if (string.IsNullOrWhiteSpace(_trackedLogin?.User?.FullName))
        {
            _fullNameValidation!.Text = "Full name is required";
            isValid = false;
        }

        // Validate Phone Number
        if (_trackedLogin?.User?.PhoneNumber != null)
        {
            if (!Regex.IsMatch(_trackedLogin.User.PhoneNumber, @"^\+?[\d\s-]{10,}$"))
            {
                _phoneValidation!.Text = "Invalid phone number format";
                isValid = false;
            }
        }

        // Validate Email
        if (_trackedLogin?.User?.Email != null)
        {
            if (!Regex.IsMatch(_trackedLogin.User.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                _emailValidation!.Text = "Invalid email format";
                isValid = false;
            }
        }

        // Validate Login
        if (string.IsNullOrWhiteSpace(_trackedLogin?.Login1))
        {
            _loginValidation!.Text = "Login is required";
            isValid = false;
        }

        // Validate Password
        if (!string.IsNullOrEmpty(_trackedLogin?.Password) && _trackedLogin.Password.Length < 6)
        {
            _passwordValidation!.Text = "Password must be at least 6 characters";
            isValid = false;
        }

        return isValid;
    }

    private void ClearValidationMessages()
    {
        _fullNameValidation!.Text = "";
        _phoneValidation!.Text = "";
        _emailValidation!.Text = "";
        _loginValidation!.Text = "";
        _passwordValidation!.Text = "";
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_db == null || _trackedLogin == null)
            {
                ShowError("No data to save");
                return;
            }

            if (!ValidateInputs())
            {
                return;
            }

            // Check if login is unique
            var existingLogin = await _db.Logins
                .FirstOrDefaultAsync(l => l.Login1 == _trackedLogin.Login1 && l.IdLogins != _trackedLogin.IdLogins);
            
            if (existingLogin != null)
            {
                _loginValidation!.Text = "This login is already taken";
                return;
            }

            await _db.SaveChangesAsync();
            ShowMessage("Profile updated successfully");
        }
        catch (Exception ex)
        {
            ShowError("Failed to save changes: " + ex.Message);
        }
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        if (_originalLogin != null)
        {
            _trackedLogin = new Login
            {
                IdLogins = _originalLogin.IdLogins,
                Login1 = _originalLogin.Login1,
                Password = _originalLogin.Password,
                User = new User
                {
                    FullName = _originalLogin.User.FullName,
                    PhoneNumber = _originalLogin.User.PhoneNumber,
                    Email = _originalLogin.User.Email,
                    Description = _originalLogin.User.Description,
                    Role = _originalLogin.User.Role
                }
            };
            DataContext = _trackedLogin;
            ClearValidationMessages();
        }
    }

    private void ShowError(string message)
    {
     
    }

    private void ShowMessage(string message)
    {
       
    }
}