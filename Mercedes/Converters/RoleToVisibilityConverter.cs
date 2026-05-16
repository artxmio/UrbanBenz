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
        var param = parameter?.ToString() ?? "";
        
        if (param.StartsWith("!"))
        {
            // Отрицание - скрыть для указанной роли
            var roleStr = param.Substring(1);
            if (Enum.TryParse<Role>(roleStr, out var role))
            {
                return SessionService.IsLoggedIn && SessionService.CurrentUser?.Role == role
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }
        else if (param == "Admin")
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