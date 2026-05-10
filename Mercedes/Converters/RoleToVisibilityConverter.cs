using Mercedes.Data.Enums;
using Mercedes.Services;
using System;
using System.Windows;

namespace Mercedes.Converters;

public class RoleToVisibilityConverter : System.Windows.Data.IValueConverter
{
    private static readonly ISessionService SessionService = ServiceInstances.Session;

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (parameter?.ToString() == "Admin")
        {
            return SessionService.IsLoggedIn && SessionService.CurrentUser?.Role == Role.Admin
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}