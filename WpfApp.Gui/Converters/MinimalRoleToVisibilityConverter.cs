using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WpfApp.Interfaces.Enums;

namespace WpfApp.Gui.Converters
{
    public class MinimalRoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parsed = Enum.TryParse((string)parameter, out Role parsedRole);

            if (value is List<Role> && parsed)
            {
                return ((List<Role>) value).Any(r => r <= parsedRole) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}