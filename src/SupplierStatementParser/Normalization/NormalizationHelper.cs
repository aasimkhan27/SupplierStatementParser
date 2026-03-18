using System.Globalization;

namespace SupplierStatementParser.Normalization;

/// <summary>
/// Normalization helpers for statement fields.
/// </summary>
public static class NormalizationHelper
{
    private static readonly string[] DateFormats =
    [
        "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd",
        "dd MMM yyyy", "d MMM yyyy", "dd MMMM yyyy", "d MMMM yyyy", "MM/dd/yyyy", "M/d/yyyy"
    ];

    public static string NormalizeKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var chars = value.Trim().ToLowerInvariant().Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray();
        return string.Join(' ', new string(chars).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    public static DateOnly? ParseDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (DateOnly.TryParseExact(raw.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var date))
        {
            return date;
        }

        if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        return null;
    }

    public static decimal? ParseDecimal(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var normalized = raw.Trim()
            .Replace(",", string.Empty, StringComparison.Ordinal)
            .Replace("$", string.Empty, StringComparison.Ordinal)
            .Replace("€", string.Empty, StringComparison.Ordinal)
            .Replace("£", string.Empty, StringComparison.Ordinal)
            .Replace("(", "-", StringComparison.Ordinal)
            .Replace(")", string.Empty, StringComparison.Ordinal);

        return decimal.TryParse(normalized, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }

    public static string? DetectCurrency(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var trimmed = raw.Trim();
        if (trimmed.Contains("USD", StringComparison.OrdinalIgnoreCase) || trimmed.Contains('$')) return "USD";
        if (trimmed.Contains("EUR", StringComparison.OrdinalIgnoreCase) || trimmed.Contains('€')) return "EUR";
        if (trimmed.Contains("GBP", StringComparison.OrdinalIgnoreCase) || trimmed.Contains('£')) return "GBP";
        if (trimmed.Length == 3 && trimmed.All(char.IsLetter)) return trimmed.ToUpperInvariant();
        return null;
    }
}
