using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Mercedes.Data.Models;

namespace Mercedes.Converters;

public class ImagePathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Car car)
        {
            try
            {
                var mainImage = car.Images?.FirstOrDefault(i => i.IsMain) ?? car.Images?.FirstOrDefault();
                
                if (mainImage != null && !string.IsNullOrEmpty(mainImage.Path))
                {
                    var basePath = AppDomain.CurrentDomain.BaseDirectory;
                    var imagePath = mainImage.Path.TrimStart('/');
                    var fullPath = System.IO.Path.Combine(basePath, imagePath);
                    
                    if (System.IO.File.Exists(fullPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }
        
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}