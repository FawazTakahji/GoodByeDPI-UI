using System;
using System.Globalization;
using System.Reflection;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Converters;

namespace GoodByeDPI.UI.Converters;

public class BooleanToIconConverter : IValueConverter
{
    private readonly IconTypeConverter _iconTypeConverter = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolean)
        {
            throw new ArgumentException("Value must be of type bool");
        }

        string assembly = Assembly.GetExecutingAssembly().GetName().Name!;
        string path = $"avares://{assembly}/Assets/{(boolean ? "green.png" : "red.png")}";
        return _iconTypeConverter.ConvertFrom(path);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}