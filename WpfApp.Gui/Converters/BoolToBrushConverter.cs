using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp.Gui.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush IfTrue { get; set; } = Brushes.Green;
        public Brush IfFalse { get; set; } = Brushes.Red;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool) value ? IfTrue : IfFalse;
            }

            return DependencyProperty.UnsetValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}