using System.Globalization;

namespace QuickLook.MediaInfo.Builder;

/// <summary>
/// Parsing helpers shared across stream builders.
/// </summary>
public static class TagBuilderHelper
{
    private static readonly NumberFormatInfo _number = new() { NumberDecimalSeparator = "." };

    private static readonly Dictionary<string, bool> BoolValues = new(StringComparer.OrdinalIgnoreCase)
    {
        { "1", true }, { "0", false }, { "y", true }, { "n", false },
        { "yes", true }, { "no", false }, { "t", true }, { "f", false },
        { "true", true }, { "false", false },
    };

    public static bool TryGetBool(this string source, out bool result) =>
        BoolValues.TryGetValue(source, out result);

    public static bool TryGetInt(this string source, out int result) =>
        int.TryParse(source, out result);

    public static bool TryGetInt(this string source, out object result)
    {
        bool ok = int.TryParse(source, out int v);
        result = v;
        return ok;
    }

    public static bool TryGetLong(this string source, out long result) =>
        long.TryParse(source, out result);

    public static bool TryGetLong(this string source, out object result)
    {
        bool ok = long.TryParse(source, out long v);
        result = v;
        return ok;
    }

    public static bool TryGetDouble(this string source, out double result) =>
        double.TryParse(source, NumberStyles.AllowDecimalPoint, _number, out result);

    public static bool TryGetDouble(this string source, out object result)
    {
        bool ok = double.TryParse(source, NumberStyles.AllowDecimalPoint, _number, out double v);
        result = v;
        return ok;
    }

    public static bool TryGetTimeSpan(this string source, out TimeSpan result)
    {
        if (double.TryParse(source, NumberStyles.AllowDecimalPoint, _number, out double ms))
        {
            result = TimeSpan.FromMilliseconds(ms);
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryGetString(this string source, out string result)
    {
        result = source;
        return !string.IsNullOrEmpty(source);
    }

    public static bool TryGetString(this string source, out object result)
    {
        result = source;
        return !string.IsNullOrEmpty(source);
    }
}
