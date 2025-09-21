using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GoodByeDPI.UI.Converters;

public class BooleanToStartStopConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolean)
        {
            throw new ArgumentException("Value must be of type bool");
        }

        return boolean ? "Stop" : "Start";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}