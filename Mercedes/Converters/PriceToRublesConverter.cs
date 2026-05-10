using System.Globalization;
using System.Windows.Data;

namespace Mercedes.Converters;

public class PriceToRublesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal price)
        {
            return $"{price:N0} BYN";
        }
        return "0 BYN";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}