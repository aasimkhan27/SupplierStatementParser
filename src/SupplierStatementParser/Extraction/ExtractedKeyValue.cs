using SupplierStatementParser.Models;

namespace SupplierStatementParser.Extraction;

/// <summary>
/// Represents a key-value pair extracted from Textract forms.
/// </summary>
public sealed record ExtractedKeyValue(string Key, RawFieldValue Value);
