using System.Globalization;

namespace InvestYes.Infrastructure.ExternalApis.FundsExplorer;

public static class FundsExplorerDictionaryExtensions
{
    public static string? GetString(this Dictionary<string, string> fields, string key)
    {
        return fields.TryGetValue(key, out var value) ? value : null;
    }

    public static decimal GetDecimal(this Dictionary<string, string> fields, string key)
    {
        if (!fields.TryGetValue(key, out var value))
            return 0;

        value = value
            .Replace("%", "")
            .Replace(".", "")
            .Replace(",", ".");

        return decimal.TryParse(
            value,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var result)
            ? result
            : 0;
    }

    public static int GetInt(this Dictionary<string, string> fields, string key)
    {
        if (!fields.TryGetValue(key, out var value))
            return 0;

        value = value.Replace(".", "");

        return int.TryParse(value, out var result) ? result : 0;
    }

    public static long GetLong(this Dictionary<string, string> fields, string key)
    {
        if (!fields.TryGetValue(key, out var value))
            return 0;

        value = value.Replace(".", "");

        return long.TryParse(value, out var result) ? result : 0;
    }
}
