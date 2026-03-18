using SupplierStatementParser.Models;

namespace SupplierStatementParser.Extraction;

/// <summary>
/// Represents a reconstructed Textract table.
/// </summary>
public sealed class ExtractedTable
{
    public string? TableId { get; init; }

    public IReadOnlyList<string> Headers { get; init; } = Array.Empty<string>();

    public IReadOnlyList<ExtractedTableRow> Rows { get; init; } = Array.Empty<ExtractedTableRow>();
}

/// <summary>
/// Represents a reconstructed table row.
/// </summary>
public sealed class ExtractedTableRow
{
    public int RowIndex { get; init; }

    public IReadOnlyDictionary<string, RawFieldValue> Values { get; init; } = new Dictionary<string, RawFieldValue>(StringComparer.OrdinalIgnoreCase);
}
