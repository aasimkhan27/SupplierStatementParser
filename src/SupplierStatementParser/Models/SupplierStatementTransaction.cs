namespace SupplierStatementParser.Models;

/// <summary>
/// Represents a single supplier statement transaction row.
/// </summary>
public sealed class SupplierStatementTransaction
{
    public int RowNumber { get; init; }

    public ParsedField<DateOnly>? Date { get; init; }

    public ParsedField<string>? Reference { get; init; }

    public ParsedField<string>? Description { get; init; }

    public MoneyValue? Debit { get; init; }

    public MoneyValue? Credit { get; init; }

    public MoneyValue? Balance { get; init; }

    public IReadOnlyDictionary<string, RawFieldValue> UnknownColumns { get; init; } = new Dictionary<string, RawFieldValue>(StringComparer.OrdinalIgnoreCase);
}
