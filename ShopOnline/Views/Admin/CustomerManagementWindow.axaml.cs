using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShopOnline.Data;
using ShopOnline.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ShopOnline;

public partial class CustomerManagementWindow : Window
{
    public CustomerManagementWindow()
    {
        InitializeComponent();
        var dataContextLogin = new Login();
        dataContextLogin.User = new User();

        // Найти роль покупателя ("Покупатель")
        var customerRole = App.DbContext.Roles.FirstOrDefault(r => EF.Functions.Like(r.NameRole, "Покупатель"));
        if (customerRole != null)
        {
            dataContextLogin.User.Role = customerRole;
            dataContextLogin.User.RoleId = customerRole.IdRoles;
        }

        DataContext = dataContextLogin;

        if (ContextData.selectedLogin1InMainWindow == null) return;
        DataContext = ContextData.selectedLogin1InMainWindow;
    }

    private void SaveButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(FullNameText.Text)) return;
        if (string.IsNullOrEmpty(PhoneNumberText.Text)) return;
        if (string.IsNullOrEmpty(DescriptionText.Text)) return;
        if (string.IsNullOrEmpty(LoginText.Text)) return;
        if (string.IsNullOrEmpty(PasswordText.Text)) return;
        if (string.IsNullOrEmpty(EmailText.Text)) return;

        if (ContextData.selectedLogin1InMainWindow != null)
        {
            var IdLogin = ContextData.selectedLogin1InMainWindow.IdLogins;

            var thisLogin = App.DbContext.Logins.FirstOrDefault(x => x.IdLogins == ContextData.selectedLogin1InMainWindow.IdLogins);
            var thisUser = App.DbContext.Logins.FirstOrDefault(x => x.UserId == ContextData.selectedLogin1InMainWindow.IdLogins);

            if (thisLogin == null) return;

            var thisContextLogin = DataContext as Login;
            thisLogin = thisContextLogin;
        }
        else
        {
            var thisContextLogin = DataContext as Login;

            App.DbContext.Logins.Add(thisContextLogin);
            App.DbContext.SaveChanges();
        }
        App.DbContext.SaveChanges();
        this.Close();
    }
}