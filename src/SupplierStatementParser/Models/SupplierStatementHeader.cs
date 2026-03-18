namespace SupplierStatementParser.Models;

/// <summary>
/// Represents the parsed supplier statement header fields.
/// </summary>
public sealed class SupplierStatementHeader
{
    public ParsedField<string>? SupplierName { get; init; }

    public ParsedField<string>? SupplierAccountNumber { get; init; }

    public ParsedField<string>? StatementNumber { get; init; }

    public ParsedField<DateOnly>? StatementDate { get; init; }

    public ParsedField<string>? Currency { get; init; }
}
