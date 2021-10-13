using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Waterdrop.Converters
{
    public class DateTimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime date = (DateTime)value;
            switch (parameter.ToString())
            {
                case "Short":
                    return date.ToString("d");

                case "Long":
                    return date.ToString("D");

                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
