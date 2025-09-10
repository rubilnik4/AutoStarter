using System.Globalization;
using System.Windows.Data;

namespace AutoStarter.Common.Converters;

public sealed class GreaterThanConverter : IValueConverter
{
    public double Threshold { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return false;

        var val = System.Convert.ToDouble(value, culture);
        var threshold = parameter != null 
            ? System.Convert.ToDouble(parameter, culture) 
            : Threshold;

        return val > threshold;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}