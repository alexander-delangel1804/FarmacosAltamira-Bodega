using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Altamira.Bodega.Presentation.Resources;

public sealed class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return Visibility.Collapsed;

        var inverse = parameter?.ToString()?.Equals("inverse", StringComparison.OrdinalIgnoreCase) == true;
        return inverse ? (boolValue ? Visibility.Collapsed : Visibility.Visible) : (boolValue ? Visibility.Visible : Visibility.Collapsed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            var inverse = parameter?.ToString()?.Equals("inverse", StringComparison.OrdinalIgnoreCase) == true;
            return inverse ? visibility != Visibility.Visible : visibility == Visibility.Visible;
        }
        return false;
    }
}
