using System.Globalization;

namespace HereForYou.Converters;

public class BoolToResponseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool responded)
        {
            return responded ? "✅ הגיב" : "❌ לא הגיב";
        }

        return "❌ לא הגיב";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
