using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WpfApp.Interfaces.Enums;

namespace WpfApp.Gui.Converters
{
    public class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parsed = Enum.TryParse((string)parameter, out Role parsedRole);

            if (value is List<Role> && parsed)
            {
                return ((List<Role>) value).Contains(parsedRole) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}