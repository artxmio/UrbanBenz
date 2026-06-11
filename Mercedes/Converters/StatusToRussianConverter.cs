using Mercedes.Data.Enums;
using Mercedes.Data.Models;
using System.Globalization;
using System.Windows.Data;

namespace Mercedes.Converters;

public class StatusToRussianConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SaleStatus status)
        {
            return status switch
            {
                SaleStatus.Pending => "Ожидает",
                SaleStatus.Completed => "Выполнено",
                SaleStatus.Cancelled => "Отменено",
                _ => status.ToString()
            };
        }

        if(value is TestDriveStatus driveStatus)
        {
            return driveStatus switch
            {
                TestDriveStatus.Pending => "Ожидает",
                TestDriveStatus.Confirmed => "Подтверждено",
                TestDriveStatus.Completed => "Выполнено",
                TestDriveStatus.Cancelled => "Отменено",
                _ => driveStatus.ToString()
            };
        }

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
