namespace SupplierStatementParser.Configuration;

/// <summary>
/// Options for supplier statement parsing, including supplier-specific aliases.
/// </summary>
public sealed class SupplierStatementParserOptions
{
    private readonly Dictionary<SupplierStatementField, HashSet<string>> _aliases = new();

    public SupplierStatementParserOptions()
    {
        SeedDefaults();
    }

    /// <summary>
    /// Gets known aliases for a logical field.
    /// </summary>
    public IReadOnlyCollection<string> GetAliases(SupplierStatementField field)
        => _aliases.TryGetValue(field, out var values) ? values : Array.Empty<string>();

    /// <summary>
    /// Adds aliases for a field.
    /// </summary>
    public SupplierStatementParserOptions AddAliases(SupplierStatementField field, params string[] aliases)
    {
        ArgumentNullException.ThrowIfNull(aliases);

        if (!_aliases.TryGetValue(field, out var set))
        {
            set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _aliases[field] = set;
        }

        foreach (var alias in aliases.Where(a => !string.IsNullOrWhiteSpace(a)))
        {
            set.Add(alias.Trim());
        }

        return this;
    }

    private void SeedDefaults()
    {
        AddAliases(SupplierStatementField.SupplierName, "supplier", "supplier name", "vendor", "vendor name", "account name");
        AddAliases(SupplierStatementField.SupplierAccountNumber, "supplier account", "supplier account number", "account number", "vendor account", "vendor no");
        AddAliases(SupplierStatementField.StatementNumber, "statement number", "statement no", "document number", "reference number");
        AddAliases(SupplierStatementField.StatementDate, "statement date", "date", "document date", "as at date");
        AddAliases(SupplierStatementField.OpeningBalance, "opening balance", "balance brought forward", "b/f balance", "previous balance");
        AddAliases(SupplierStatementField.ClosingBalance, "closing balance", "balance carried forward", "c/f balance", "ending balance");
        AddAliases(SupplierStatementField.TotalDebit, "total debit", "debits", "charges", "total charges");
        AddAliases(SupplierStatementField.TotalCredit, "total credit", "credits", "payments", "total payments");
        AddAliases(SupplierStatementField.Currency, "currency", "statement currency", "curr");
        AddAliases(SupplierStatementField.TransactionDate, "date", "transaction date", "posting date");
        AddAliases(SupplierStatementField.TransactionReference, "reference", "ref", "invoice", "document", "doc no");
        AddAliases(SupplierStatementField.TransactionDescription, "description", "details", "narration", "particulars");
        AddAliases(SupplierStatementField.TransactionDebit, "debit", "charge", "invoice amount");
        AddAliases(SupplierStatementField.TransactionCredit, "credit", "payment", "receipt");
        AddAliases(SupplierStatementField.TransactionBalance, "balance", "running balance", "outstanding balance");
    }
}
