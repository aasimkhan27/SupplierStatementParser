namespace SupplierStatementParser.Models;

/// <summary>
/// Represents the fully parsed supplier statement.
/// </summary>
public sealed class SupplierStatementData
{
    public SupplierStatementHeader Header { get; init; } = new();

    public SupplierStatementSummary Summary { get; init; } = new();

    public IReadOnlyList<SupplierStatementTransaction> Transactions { get; init; } = Array.Empty<SupplierStatementTransaction>();

    public IReadOnlyDictionary<string, RawFieldValue> UnknownFields { get; init; } = new Dictionary<string, RawFieldValue>(StringComparer.OrdinalIgnoreCase);
}
