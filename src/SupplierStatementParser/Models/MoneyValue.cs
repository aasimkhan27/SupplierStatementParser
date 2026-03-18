namespace SupplierStatementParser.Models;

/// <summary>
/// Represents a parsed monetary field while preserving source metadata.
/// </summary>
public sealed class MoneyValue
{
    /// <summary>
    /// Initializes a new money value.
    /// </summary>
    public MoneyValue(decimal? amount, string? currency, string? rawValue, float? confidence, string? sourceType, string? sourceId = null)
    {
        Amount = amount;
        Currency = currency;
        RawValue = rawValue;
        Confidence = confidence;
        SourceType = sourceType;
        SourceId = sourceId;
    }

    /// <summary>
    /// Gets the parsed amount.
    /// </summary>
    public decimal? Amount { get; }

    /// <summary>
    /// Gets the parsed currency.
    /// </summary>
    public string? Currency { get; }

    /// <summary>
    /// Gets the raw extracted text.
    /// </summary>
    public string? RawValue { get; }

    /// <summary>
    /// Gets the Textract confidence.
    /// </summary>
    public float? Confidence { get; }

    /// <summary>
    /// Gets the source type used to extract the value.
    /// </summary>
    public string? SourceType { get; }

    /// <summary>
    /// Gets the source block identifier.
    /// </summary>
    public string? SourceId { get; }
}
