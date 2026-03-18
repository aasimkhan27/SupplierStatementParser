namespace SupplierStatementParser.Configuration;

/// <summary>
/// Enumerates logical supplier statement field names.
/// </summary>
public enum SupplierStatementField
{
    SupplierName,
    SupplierAccountNumber,
    StatementNumber,
    StatementDate,
    OpeningBalance,
    ClosingBalance,
    TotalDebit,
    TotalCredit,
    Currency,
    TransactionDate,
    TransactionReference,
    TransactionDescription,
    TransactionDebit,
    TransactionCredit,
    TransactionBalance
}
