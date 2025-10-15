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
    private AppDbContext? _db;
    private Login? _trackedLogin;

    public AdminProfileUserControl()
    {
        InitializeComponent();
        LoadProfile();
    }

   

  
	private void LoadProfile()
	{
     
     _db?.Dispose();

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
            DataContext = _trackedLogin;
		
		
	}

    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        
            if (_db == null || _trackedLogin == null)
            {
                return;
            }
            _db.SaveChanges();
       
    }
}