using SupplierStatementParser.Configuration;
using SupplierStatementParser.Extraction;
using SupplierStatementParser.Models;
using SupplierStatementParser.Normalization;

namespace SupplierStatementParser.Mapping;

/// <summary>
/// Maps extracted raw fields and rows into clean domain models.
/// </summary>
public sealed class SupplierStatementMapper
{
    public SupplierStatementData Map(
        IReadOnlyDictionary<string, RawFieldValue> formFields,
        IReadOnlyDictionary<string, RawFieldValue> queryFields,
        IReadOnlyList<ExtractedTable> tables,
        SupplierStatementParserOptions options,
        ICollection<ParseIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(formFields);
        ArgumentNullException.ThrowIfNull(queryFields);
        ArgumentNullException.ThrowIfNull(tables);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(issues);

        var combined = MergeFields(formFields, queryFields);
        var unknown = new Dictionary<string, RawFieldValue>(combined, StringComparer.OrdinalIgnoreCase);

        RawFieldValue? TakeField(SupplierStatementField field)
        {
            foreach (var alias in options.GetAliases(field))
            {
                if (combined.TryGetValue(NormalizationHelper.NormalizeKey(alias), out var value))
                {
                    unknown.Remove(NormalizationHelper.NormalizeKey(value.Name));
                    return value;
                }
            }

            return null;
        }

        var currencyField = TakeField(SupplierStatementField.Currency);
        var fallbackCurrency = NormalizationHelper.DetectCurrency(currencyField?.Value);

        var data = new SupplierStatementData
        {
            Header = new SupplierStatementHeader
            {
                SupplierName = ToStringField(TakeField(SupplierStatementField.SupplierName)),
                SupplierAccountNumber = ToStringField(TakeField(SupplierStatementField.SupplierAccountNumber)),
                StatementNumber = ToStringField(TakeField(SupplierStatementField.StatementNumber)),
                StatementDate = ToDateField(TakeField(SupplierStatementField.StatementDate), issues, "statement_date"),
                Currency = ToStringField(currencyField)
            },
            Summary = new SupplierStatementSummary
            {
                OpeningBalance = ToMoney(TakeField(SupplierStatementField.OpeningBalance), fallbackCurrency, issues, "opening_balance"),
                ClosingBalance = ToMoney(TakeField(SupplierStatementField.ClosingBalance), fallbackCurrency, issues, "closing_balance"),
                TotalDebit = ToMoney(TakeField(SupplierStatementField.TotalDebit), fallbackCurrency, issues, "total_debit"),
                TotalCredit = ToMoney(TakeField(SupplierStatementField.TotalCredit), fallbackCurrency, issues, "total_credit")
            },
            Transactions = MapTransactions(tables, options, fallbackCurrency, issues),
            UnknownFields = unknown
        };

        Validate(data, issues);
        return data;
    }

    private static IReadOnlyDictionary<string, RawFieldValue> MergeFields(
        IReadOnlyDictionary<string, RawFieldValue> formFields,
        IReadOnlyDictionary<string, RawFieldValue> queryFields)
    {
        var merged = new Dictionary<string, RawFieldValue>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in formFields.Values.Concat(queryFields.Values))
        {
            merged[NormalizationHelper.NormalizeKey(kvp.Name)] = kvp;
        }

        return merged;
    }

    private static IReadOnlyList<SupplierStatementTransaction> MapTransactions(
        IReadOnlyList<ExtractedTable> tables,
        SupplierStatementParserOptions options,
        string? currency,
        ICollection<ParseIssue> issues)
    {
        var bestTable = tables
            .OrderByDescending(t => ScoreTable(t, options))
            .ThenByDescending(t => t.Rows.Count)
            .FirstOrDefault();

        if (bestTable is null)
        {
            issues.Add(new ParseIssue("table.missing", "No transaction table was detected.", ParseSeverity.Warning, "TABLES"));
            return Array.Empty<SupplierStatementTransaction>();
        }

        return bestTable.Rows.Select(row => MapTransactionRow(row, options, currency, issues)).ToArray();
    }

    private static int ScoreTable(ExtractedTable table, SupplierStatementParserOptions options)
    {
        var normalizedHeaders = table.Headers.Select(NormalizationHelper.NormalizeKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var wanted = new[]
        {
            SupplierStatementField.TransactionDate,
            SupplierStatementField.TransactionReference,
            SupplierStatementField.TransactionDescription,
            SupplierStatementField.TransactionDebit,
            SupplierStatementField.TransactionCredit,
            SupplierStatementField.TransactionBalance
        };

        return wanted.Sum(field => options.GetAliases(field).Select(NormalizationHelper.NormalizeKey).Any(normalizedHeaders.Contains) ? 1 : 0);
    }

    private static SupplierStatementTransaction MapTransactionRow(
        ExtractedTableRow row,
        SupplierStatementParserOptions options,
        string? currency,
        ICollection<ParseIssue> issues)
    {
        RawFieldValue? Match(SupplierStatementField field)
        {
            foreach (var alias in options.GetAliases(field))
            {
                var match = row.Values.FirstOrDefault(v => string.Equals(NormalizationHelper.NormalizeKey(v.Key), NormalizationHelper.NormalizeKey(alias), StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(match.Key))
                {
                    return match.Value;
                }
            }

            return null;
        }

        var knownKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        void Track(RawFieldValue? value)
        {
            if (value is not null)
            {
                knownKeys.Add(value.Name);
            }
        }

        var date = Match(SupplierStatementField.TransactionDate); Track(date);
        var reference = Match(SupplierStatementField.TransactionReference); Track(reference);
        var description = Match(SupplierStatementField.TransactionDescription); Track(description);
        var debit = Match(SupplierStatementField.TransactionDebit); Track(debit);
        var credit = Match(SupplierStatementField.TransactionCredit); Track(credit);
        var balance = Match(SupplierStatementField.TransactionBalance); Track(balance);

        var unknown = row.Values.Where(v => !knownKeys.Contains(v.Key)).ToDictionary(v => v.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);
        var mapped = new SupplierStatementTransaction
        {
            RowNumber = row.RowIndex,
            Date = ToDateField(date, issues, $"transaction[{row.RowIndex}].date"),
            Reference = ToStringField(reference),
            Description = ToStringField(description),
            Debit = ToMoney(debit, currency, issues, $"transaction[{row.RowIndex}].debit"),
            Credit = ToMoney(credit, currency, issues, $"transaction[{row.RowIndex}].credit"),
            Balance = ToMoney(balance, currency, issues, $"transaction[{row.RowIndex}].balance"),
            UnknownColumns = unknown
        };

        return mapped;
    }

    private static ParsedField<string>? ToStringField(RawFieldValue? raw)
        => raw is null ? null : new ParsedField<string>(raw.Value, raw.Value, raw.Confidence, raw.SourceType, raw.SourceId);

    private static ParsedField<DateOnly>? ToDateField(RawFieldValue? raw, ICollection<ParseIssue> issues, string fieldName)
    {
        if (raw is null)
        {
            return null;
        }

        var value = NormalizationHelper.ParseDate(raw.Value);
        if (value is null && !string.IsNullOrWhiteSpace(raw.Value))
        {
            issues.Add(new ParseIssue("date.parse_failed", $"Unable to parse date for {fieldName}: '{raw.Value}'.", ParseSeverity.Warning, raw.SourceType));
        }

        return new ParsedField<DateOnly>(value, raw.Value, raw.Confidence, raw.SourceType, raw.SourceId);
    }

    private static MoneyValue? ToMoney(RawFieldValue? raw, string? fallbackCurrency, ICollection<ParseIssue> issues, string fieldName)
    {
        if (raw is null)
        {
            return null;
        }

        var amount = NormalizationHelper.ParseDecimal(raw.Value);
        var currency = NormalizationHelper.DetectCurrency(raw.Value) ?? fallbackCurrency;

        if (amount is null && !string.IsNullOrWhiteSpace(raw.Value))
        {
            issues.Add(new ParseIssue("decimal.parse_failed", $"Unable to parse decimal for {fieldName}: '{raw.Value}'.", ParseSeverity.Warning, raw.SourceType));
        }

        return new MoneyValue(amount, currency, raw.Value, raw.Confidence, raw.SourceType, raw.SourceId);
    }

    private static void Validate(SupplierStatementData data, ICollection<ParseIssue> issues)
    {
        if (string.IsNullOrWhiteSpace(data.Header.SupplierName?.Value))
        {
            issues.Add(new ParseIssue("field.missing", "Supplier name was not found.", ParseSeverity.Warning, "FORMS/QUERIES"));
        }

        if (data.Header.StatementDate?.Value is null)
        {
            issues.Add(new ParseIssue("field.missing", "Statement date was not found or parsed.", ParseSeverity.Warning, "FORMS/QUERIES"));
        }

        if (data.Summary.ClosingBalance?.Amount is null)
        {
            issues.Add(new ParseIssue("field.missing", "Closing balance was not found or parsed.", ParseSeverity.Warning, "FORMS/QUERIES"));
        }
    }
}
