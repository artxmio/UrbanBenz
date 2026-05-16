using System.Globalization;
using System.Windows.Data;

namespace Mercedes.Converters;

public class BoolToYesNoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "В наличии" : "Нет в наличии";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == "В наличии";
    }
}
