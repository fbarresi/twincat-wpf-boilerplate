using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TwinCAT;

namespace WpfApp.Gui.Converters
{
    public class ConnectionStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConnectionState)
            {
                return (ConnectionState) value == ConnectionState.Connected ? Visibility.Collapsed : Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}