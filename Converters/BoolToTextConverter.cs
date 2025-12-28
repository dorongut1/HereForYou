using System.Globalization;

namespace HereForYou.Converters;

public class BoolToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isMonitoring)
        {
            return isMonitoring ? "עצור ניטור" : "התחל ניטור";
        }

        return "התחל ניטור";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
