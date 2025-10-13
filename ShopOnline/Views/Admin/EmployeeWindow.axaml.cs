using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShopOnline.Data;
using ShopOnline.Models;
using ShopOnline.ViewModels;
using System.Linq;

namespace ShopOnline;

public partial class EmployeeWindow : Window
{
    public EmployeeWindow()
    {
        InitializeComponent();
        var dataContextLogin = new Login();
        dataContextLogin.User = new User();

        ComboRole.ItemsSource = App.DbContext.Roles.ToList();

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