using Mercedes.Data.Enums;
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

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
