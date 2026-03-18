namespace SupplierStatementParser.Models;

/// <summary>
/// Wraps a parsed field while preserving raw and confidence details.
/// </summary>
/// <typeparam name="T">Parsed value type.</typeparam>
public sealed class ParsedField<T>
{
    /// <summary>
    /// Initializes a new parsed field.
    /// </summary>
    public ParsedField(T? value, string? rawValue, float? confidence, string? sourceType, string? sourceId = null)
    {
        Value = value;
        RawValue = rawValue;
        Confidence = confidence;
        SourceType = sourceType;
        SourceId = sourceId;
    }

    /// <summary>
    /// Gets the parsed value.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the raw extracted text.
    /// </summary>
    public string? RawValue { get; }

    /// <summary>
    /// Gets the confidence score reported by Textract.
    /// </summary>
    public float? Confidence { get; }

    /// <summary>
    /// Gets the Textract extraction source type.
    /// </summary>
    public string? SourceType { get; }

    /// <summary>
    /// Gets the Textract block identifier.
    /// </summary>
    public string? SourceId { get; }
}
