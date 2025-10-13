using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System;
using System.Linq;

namespace ShopOnline;

public partial class AdminProfileUserControl : UserControl
{
 

    public AdminProfileUserControl()
    {
        InitializeComponent();

        LoadData();
    }

    public void RefreshData()
    {
        LoadData();
    }

    private void LoadData()
    {
        var currentUser = ContextData.CurrentUser;

        if (currentUser != null)
        {
            var user = currentUser
                .Include(u => u.Role)
                .Include(u => u.Logins)
                .FirstOrDefault(u => u.IdUsers == currentUser.IdUsers);

            if (user != null)
            {
                FullNameTextBox.Text = user.FullName ?? "";
                PhoneNumberTextBox.Text = user.PhoneNumber ?? "";
                DescriptionTextBox.Text = user.Description ?? "";
                EmailTextBox.Text = user.Email ?? "";
                RoleTextBox.Text = user.Role?.NameRole ?? "";

                var login = user.Logins.FirstOrDefault();
                if (login != null)
                {
                    LoginTextBox.Text = login.Login1 ?? "";
                    PasswordTextBox.Text = login.Password ?? "";
                }
            }
        }
    }

  
}