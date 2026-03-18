namespace SupplierStatementParser.Models;

/// <summary>
/// Represents parsed summary values from the statement.
/// </summary>
public sealed class SupplierStatementSummary
{
    public MoneyValue? OpeningBalance { get; init; }

    public MoneyValue? ClosingBalance { get; init; }

    public MoneyValue? TotalDebit { get; init; }

    public MoneyValue? TotalCredit { get; init; }
}
