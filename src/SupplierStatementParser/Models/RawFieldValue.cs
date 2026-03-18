namespace SupplierStatementParser.Models;

/// <summary>
/// Captures the raw value and metadata extracted from Textract.
/// </summary>
public sealed record RawFieldValue(
    string Name,
    string? Value,
    float? Confidence,
    string? SourceType,
    string? SourceId = null,
    string? NormalizedName = null);
