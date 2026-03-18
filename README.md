# SupplierStatementParser

A .NET 8 class library for parsing AWS Textract Supplier Statement of Account documents using **FORMS**, **TABLES**, **QUERIES**, and **LAYOUT**.

## Features

- Strong domain models for header, summary, and transaction rows.
- Preserves raw values, confidence, source type, and unknown fields.
- Supports supplier-specific field alias mapping.
- Includes helpers for block indexing, text reconstruction, normalization, validation, and mapping.

## Example usage

```csharp
using Amazon.Textract.Model;
using SupplierStatementParser;
using SupplierStatementParser.Configuration;

IReadOnlyCollection<Block> blocks = GetTextractBlocksSomehow();

var options = new SupplierStatementParserOptions()
    .AddAliases(SupplierStatementField.SupplierName, "vendor")
    .AddAliases(SupplierStatementField.TransactionReference, "doc no");

var parser = new SupplierStatementParser();
var result = parser.Parse(blocks, options);

Console.WriteLine($"Supplier: {result.Data.Header.SupplierName?.Value}");
Console.WriteLine($"Closing balance: {result.Data.Summary.ClosingBalance?.Amount}");

foreach (var issue in result.Issues)
{
    Console.WriteLine($"[{issue.Severity}] {issue.Code}: {issue.Message}");
}
```
